using FluentValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Business.Services
{
    public class QuadroServices : IQuadroServices
    {
        ICongregacaoRepository _repositoryCongregacao;
        IControleListaRepository _repositoryControleLista;
        IDataEventoRepository _repositoryDataEvento;
        IIrmaoRepository _repositoryIrmao;
        IQuadroDetalheRepository _repositoryQuadroDetalhe;
        IQuadroRepository _repositoryQuadro;
        ITipoListaRepository _repositoryTipoLista;

        public QuadroServices(ICongregacaoRepository repositoryCongregacao,
                              IControleListaRepository repositoryControleLista,
                              IDataEventoRepository repositoryDataEvento,
                              IIrmaoRepository repositoryIrmao,
                              IQuadroDetalheRepository repositoryQuadroDetalhe,
                              IQuadroRepository repositoryQuadro,
                              ITipoListaRepository repositoryTipoLista)
        {
            _repositoryCongregacao = repositoryCongregacao;
            _repositoryControleLista = repositoryControleLista;
            _repositoryDataEvento = repositoryDataEvento;
            _repositoryIrmao = repositoryIrmao;
            _repositoryQuadroDetalhe = repositoryQuadroDetalhe;
            _repositoryQuadro = repositoryQuadro;
            _repositoryTipoLista = repositoryTipoLista;
        }

        public IReadOnlyCollection<Notification> GeraLista()
        {
            var model = new Quadro();
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var congregacoes = _repositoryCongregacao.ListAll(ref unitOfWork);
                    if (congregacoes == null)
                    {
                        model.AddNotification("Congregacao", "Não existe nenhuma congregação cadastrada");
                        unitOfWork.Rollback();
                        return model.Notifications;
                    }

                    foreach (var item in congregacoes)
                    {
                        DateTime dataInicioLista;
                        if (DateTime.Now.Date < Convert.ToDateTime(item.DataPrimeiraLista).Date)
                            dataInicioLista = Convert.ToDateTime(item.DataPrimeiraLista).Date;
                        else
                        {
                            var ultimareuniao = _repositoryQuadro.ObterUltimoQuadro(ref unitOfWork, item.Codigo);
                            dataInicioLista = ultimareuniao.DataFimLista.Date.AddDays(1); 
                        }

                        if (DateTime.Now.Date < dataInicioLista.Date.AddDays(item.DiasAntecedenciaGerarLista * -1))
                        {
                            model.AddNotification("Congregacao", $"A lista somente pode ser gerada a partir de + " +
                                $"'{dataInicioLista.Date.AddDays(item.DiasAntecedenciaGerarLista * -1).Date}'");
                            unitOfWork.Rollback();
                            return model.Notifications;
                        }

                        var tipolistas = _repositoryTipoLista.ListAll(ref unitOfWork, item.Codigo);
                        int quadro = 0;
                        int codQuadro = 0;
                        if (tipolistas.Count() > 0)
                        {
                            quadro = _repositoryQuadro.ObterCodigoProximoQuadro(ref unitOfWork);
                            // Incluir / retirar irmãos da lista / atualiza designações
                            AtualizarControleLista(ref unitOfWork, item.Codigo);
                        }
                        DateTime dataFinalLista = DateTime.MinValue;

                        foreach (var itemTipoLista in tipolistas)
                        {
                            DateTime dataControle = dataInicioLista;
                            _repositoryControleLista.BackupListaAtual(ref unitOfWork, (int)itemTipoLista.Codigo, dataInicioLista);
                            codQuadro =_repositoryQuadro.InserirNovoQuadro(ref unitOfWork, item.Codigo, quadro, (int)itemTipoLista.Codigo);

                            int i = 0;
                            while (i < itemTipoLista.QuantidadeDatas)
                            {
                                bool assembleia = false;
                                switch (itemTipoLista.Codigo)
                                {
                                    case Core.Domain.enums.eTipoLista.Indicador:
                                    case Core.Domain.enums.eTipoLista.AudioVideo:
                                        if (dataControle.DayOfWeek == item.DiaReuniaoSentinela || dataControle.DayOfWeek == item.DiaReuniaoServico)
                                        {
                                            for (int iIndicador = 0; iIndicador < item.QuantidadeIndicadores; iIndicador++)
                                            {
                                                if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, item, codQuadro, itemTipoLista);
                                            }
                                            i++;
                                        }
                                        break;
                                    case Core.Domain.enums.eTipoLista.Microfonista:
                                        if (dataControle.DayOfWeek == item.DiaReuniaoSentinela || dataControle.DayOfWeek == item.DiaReuniaoServico)
                                        {
                                            for (int iMicrofonistas = 0; iMicrofonistas < item.QuantidadeMicrofonistas; iMicrofonistas++)
                                            {
                                                if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, item, codQuadro, itemTipoLista);
                                            }
                                            i++;
                                        }
                                        break;
                                    case Core.Domain.enums.eTipoLista.OracaoFinal:
                                        if (dataControle.DayOfWeek == item.DiaReuniaoSentinela || dataControle.DayOfWeek == item.DiaReuniaoServico)
                                        {
                                           InsereDetalheQuadro(ref unitOfWork, dataControle, item, codQuadro, itemTipoLista);
                                            i++;
                                        }
                                        break;
                                    case Core.Domain.enums.eTipoLista.LeitorJW:
                                        if (dataControle.DayOfWeek == item.DiaReuniaoSentinela)
                                        {
                                            if (dataControle <= dataFinalLista)  InsereDetalheQuadro(ref unitOfWork, dataControle, item, codQuadro, itemTipoLista);
                                            i++;
                                        }
                                        break;
                                    case Core.Domain.enums.eTipoLista.LeitorELC:
                                        if (dataControle.DayOfWeek == item.DiaReuniaoServico)
                                        {
                                            if (dataControle <= dataFinalLista) InsereDetalheQuadro(ref unitOfWork, dataControle, item, codQuadro, itemTipoLista);
                                            i++;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                dataControle = dataControle.AddDays(1);
                            }
                            if (dataFinalLista == DateTime.MinValue)
                                dataFinalLista =dataControle.AddDays(-1); //Convert.ToDateTime("2018-12-11");

                        }
                    }                                                        
                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }

            return model.Notifications;
        }

        public List<GetQuadroDesignacaoMecanica> ObterListaAtualDesignacoesMecanicas(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    int quadro = _repositoryQuadro.ObterQuadroAtual(ref unitOfWork, congregacaoId);
                    return ObterListaPorQuadroId(ref unitOfWork, quadro);
                }
                catch
                {
                    throw;
                }
            }
        }

        public List<GetQuadroDesignacaoMecanica> ObterProximaListaDesignacoesMecanicas(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    int quadro = _repositoryQuadro.ObterProximoQuadro(ref unitOfWork, congregacaoId);
                    return ObterListaPorQuadroId(ref unitOfWork, quadro);
                }
                catch
                {
                    throw;
                }
            }
        }

        public IReadOnlyCollection<Notification> RegerarListaAtual()
        {
            throw new NotImplementedException();
        }

        private List<GetQuadroDesignacaoMecanica> ObterListaPorQuadroId(ref IUnitOfWork unitOfWork, int quadro)
        {
            var lista = _repositoryQuadro.ObterListaDesignacoesMecanicas(ref unitOfWork, quadro);
            for (int i = 0; i < lista.Count; i++)
            {
                lista[i].DataFormatada = lista[i].Data.ToString("dd/MM");
                var indicadores = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.Indicador, quadro, lista[i].Data);
                lista[i].Indicadores = new List<string>();
                foreach (var item in indicadores)
                {
                    lista[i].Indicadores.Add(item.Nome);
                }

                var microfonistas = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.Microfonista, quadro, lista[i].Data);
                lista[i].Microfonistas = new List<string>();
                foreach (var item2 in microfonistas)
                {
                    lista[i].Microfonistas.Add(item2.Nome);
                }

                var somvideo = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.AudioVideo, quadro, lista[i].Data);
                lista[i].SomVideo = new List<string>();
                foreach (var item3 in somvideo)
                {
                    lista[i].SomVideo.Add(item3.Nome);
                }
            }
            return lista;
        }

        private void AtualizarControleLista(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var irmaos = _repositoryIrmao.ObterIrmaosADesativarOuAtivar(ref unitOfWork, congregacaoId);
            ControleLista lista = null;
            bool inicioLista = false;
            IEnumerable<TipoLista> tiposlistas = _repositoryTipoLista.ListAll(ref unitOfWork, congregacaoId); 
            
            foreach (var itemIrmao in irmaos)
            {

                if (itemIrmao.AtivarProximaLista)
                {
                    _repositoryIrmao.AtualizaAtivarProximaLista(ref unitOfWork, itemIrmao.Codigo);
                }
                else if (itemIrmao.DesativarProximaLista)
                {
                    _repositoryIrmao.AtualizaDesativarProximaLista(ref unitOfWork, itemIrmao.Codigo);
                }

                if (itemIrmao.AtualizarDesignacao)
                {
                    foreach (var item in tiposlistas)
                    {
                        lista = _repositoryControleLista.GetListaIrmao(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                        switch (item.Codigo)
                        {
                            case Core.Domain.enums.eTipoLista.Indicador:
                                if (itemIrmao.Indicador && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.Indicador && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.Microfonista:
                                if (itemIrmao.Microfonista && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.Microfonista && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.LeitorJW:
                                if (itemIrmao.LeitorSentinela && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.LeitorSentinela && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.OracaoFinal:
                                if (itemIrmao.OracaoFinal && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.OracaoFinal && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.LeitorELC:
                                if (itemIrmao.LeitorEstudoLivro && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.LeitorEstudoLivro && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.AudioVideo:
                                if (itemIrmao.SistemaSonoro && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = (inicioLista) ? false : true;
                                }
                                else if (!itemIrmao.SistemaSonoro && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private bool InsereDetalheQuadro(ref IUnitOfWork unitOfWork, DateTime dataControle, Congregacao item, int codigoQuadro, TipoLista itemTipoLista)
        {
            ControleLista proximoLista = null;
            bool liberouproximo = false;
            QuadroDetalhe quadrodetalhe = null;
            DataEvento evento = _repositoryDataEvento.ListByDate(ref unitOfWork, item.Codigo, dataControle);
            var ultimaReuniao = _repositoryQuadroDetalhe.ObterUltimaReuniaoValida(ref unitOfWork, item.Codigo, dataControle);
            var proximaReuniao = _repositoryQuadroDetalhe.ObterProximaReuniaoValida(ref unitOfWork, item.Codigo, dataControle);

            if (evento == null || (evento.VisitaSuperintendente && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorELC 
                && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorJW))
            {
                if (item.FolgaParticipacao && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.OracaoFinal && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.AudioVideo)
                {
                    while (proximoLista == null)
                    {
                        proximoLista = _repositoryControleLista.ObterProximoListaSemRepetirComFolga(ref unitOfWork,
                            (int)itemTipoLista.Codigo, (ultimaReuniao == null) ? dataControle.AddDays(-1) : ultimaReuniao.Data, dataControle,
                            (proximaReuniao == null) ? dataControle.AddDays(1) : proximaReuniao.Data);
                        if (proximoLista == null)
                        {
                            _repositoryControleLista.LiberaProximoLista(ref unitOfWork, (int)itemTipoLista.Codigo);
                            liberouproximo = true;
                        }
                    }
                }
                else if (itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.OracaoFinal && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.AudioVideo)
                {
                    while (proximoLista == null)
                    {
                        proximoLista = _repositoryControleLista.ObterProximoListaSemRepetirSemFolga(ref unitOfWork,
                        (int)itemTipoLista.Codigo, dataControle);

                        if (proximoLista == null)
                        {
                            _repositoryControleLista.LiberaProximoLista(ref unitOfWork, (int)itemTipoLista.Codigo);
                            liberouproximo = true;
                        }
                            
                    }
                }
                else if (itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.AudioVideo)
                {
                    while (proximoLista == null)
                    {
                        proximoLista = _repositoryControleLista.ObterProximoListaSemRepetirComFolga(ref unitOfWork,
                           (int)itemTipoLista.Codigo, (ultimaReuniao == null) ? dataControle.AddDays(-1) : ultimaReuniao.Data, dataControle,
                           (proximaReuniao == null) ? dataControle.AddDays(1) : proximaReuniao.Data);

                        if (proximoLista == null)
                        {
                            _repositoryControleLista.LiberaProximoLista(ref unitOfWork, (int)itemTipoLista.Codigo);
                            liberouproximo = true;
                        }

                    }
                }
                else
                {
                    while (proximoLista == null)
                    {
                        proximoLista = _repositoryControleLista.ObterProximoListaPodeRepetir(ref unitOfWork,
                        (int)itemTipoLista.Codigo);

                        if (proximoLista == null)
                        {
                            _repositoryControleLista.LiberaProximoLista(ref unitOfWork, (int)itemTipoLista.Codigo);
                            liberouproximo = true;
                        }
                    }
                }

                quadrodetalhe = new QuadroDetalhe() { Data = dataControle, IrmaoId = proximoLista.IrmaoId, QuadroId = codigoQuadro };
                _repositoryControleLista.AtualizaPartipacaoIrmaoLista(ref unitOfWork, (int)itemTipoLista.Codigo, proximoLista.IrmaoId, liberouproximo);
            }
            else
                quadrodetalhe = new QuadroDetalhe() { Data = dataControle, QuadroId = codigoQuadro, EventoId = evento.Codigo };

            _repositoryQuadroDetalhe.InsereDataQuadro(ref unitOfWork, quadrodetalhe);

            return (evento != null && evento.Assembleia) ? true : false;
        }
    }
}
