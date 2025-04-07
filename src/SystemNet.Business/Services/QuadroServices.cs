using FluentValidator;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Storage;
using SystemNet.Practices.Data.Storage.Models;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;
using SystemNet.Practice.Common;
using System.Diagnostics;

namespace SystemNet.Business.Services
{
    public class QuadroServices : IQuadroServices
    {
        readonly ICongregacaoRepository _repositoryCongregacao;
        readonly IControleListaRepository _repositoryControleLista;
        readonly IDataEventoRepository _repositoryDataEvento;
        readonly IIrmaoRepository _repositoryIrmao;
        readonly IQuadroDetalheRepository _repositoryQuadroDetalhe;
        readonly IQuadroRepository _repositoryQuadro;
        readonly ITipoListaRepository _repositoryTipoLista;
        readonly StorageHelper _storageAzure;


        public QuadroServices(ICongregacaoRepository repositoryCongregacao,
                              IControleListaRepository repositoryControleLista,
                              IDataEventoRepository repositoryDataEvento,
                              IIrmaoRepository repositoryIrmao,
                              IQuadroDetalheRepository repositoryQuadroDetalhe,
                              IQuadroRepository repositoryQuadro,
                              ITipoListaRepository repositoryTipoLista,
                              StorageHelper storageAzure
                              )
        {
            _repositoryCongregacao = repositoryCongregacao;
            _repositoryControleLista = repositoryControleLista;
            _repositoryDataEvento = repositoryDataEvento;
            _repositoryIrmao = repositoryIrmao;
            _repositoryQuadroDetalhe = repositoryQuadroDetalhe;
            _repositoryQuadro = repositoryQuadro;
            _repositoryTipoLista = repositoryTipoLista;
            _storageAzure = storageAzure;
        }

        public async Task<QuadroPersonalizado> NovoQuadroPersonalizado(QuadroPersonalizado model, StorageConfig config, IFormFile file)
        {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    JsonImage url = new JsonImage();
                    unitOfWork.Begin();
                    try
                    {

                        if (model.Valid)
                        {
                            if (file != null && _storageAzure.IsImage(file.FileName.Trim('\"')))
                            {
                                if (file.Length > 0)
                                 {
                                  using (var stream = file.OpenReadStream())
                                    url = await _storageAzure.Upload(stream, Guid.NewGuid() + "_" + file.FileName, config);
                                 }
                                else
                                 model.AddNotification(nameof(JsonImage.Url), "Arquivo Vazio");
                            }
                            else
                                model.AddNotification(nameof(JsonImage.Url), "Arquivo não suportado");

                            model.Url = url.Url;
                            if (model.Valid)
                             {
                                _repositoryQuadro.InserirQuadroPersonalizado(ref unitOfWork, model);
                                unitOfWork.Commit();
                             }
                            else if (model.Url != null)
                             await _storageAzure.DeleteBlobData(model.Url, config);
                    }
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        await _storageAzure.DeleteBlobData(model.Url, config);
                        throw;
                    }
                return model;
            }
        }

        public async Task<List<QuadroPersonalizado>> ApagarStorageNaoUtilizado(StorageConfig config)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var ret = _repositoryQuadro.ObterQuadrosPersonalizadosExpiradosAtivosStorage(ref unitOfWork);

                    foreach (var item in ret)
                    {
                        await _storageAzure.DeleteBlobData(item.Url, config);
                        _repositoryQuadro.AlterarStatusStorageQuadroPersonalizado(ref unitOfWork, item.Url);
                    }
                    return ret;            
                }
                catch
                {
                    throw;
                }
            }
        }

        public List<QuadroPersonalizado> ObterQuadrosPersonalizados(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repositoryQuadro.ObterQuadrosPersonalizadosAtivosPorCongregacao(ref unitOfWork, congregacaoId);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<QuadroPersonalizado> ApagarQuadroPersonalizado(int congregacaoId, string url, StorageConfig config)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = _repositoryQuadro.ObterQuadroPersonalizado(ref unitOfWork, congregacaoId, url);
                    if (model == null)
                    {
                        model = new QuadroPersonalizado();
                        model.AddNotification(nameof(JsonImage.Url), "Quadro não encontrado");
                        unitOfWork.Rollback();
                        return model;
                    }

                    await _storageAzure.DeleteBlobData(model.Url, config);
                    _repositoryQuadro.ApagarQuadroPersonalizado(ref unitOfWork, url, congregacaoId);
                    unitOfWork.Commit();
                    return model;
                }
                catch
                {
                    throw;
                }
            }
        }

        public IReadOnlyCollection<Notification> GeraLista(int congregacaoAtual)
        {
            var model = new Quadro();
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {

                    var congregacao = _repositoryCongregacao.ListAll(ref unitOfWork).ToList().Find(x => x.Codigo == congregacaoAtual);

                    if (congregacao == null)
                    {
                        model.AddNotification("Congregacao", "Não existe nenhuma congregação cadastrada");
                        unitOfWork.Rollback();
                        return model.Notifications;
                    }

                        DateTime dataInicioLista;
                        if (DateTime.Now.Date < Convert.ToDateTime(congregacao.DataPrimeiraLista).Date)
                            dataInicioLista = Convert.ToDateTime(congregacao.DataPrimeiraLista).Date;
                        else
                        {
                            var ultimareuniao = _repositoryQuadro.ObterUltimoQuadro(ref unitOfWork, congregacao.Codigo);
                            dataInicioLista = ultimareuniao.DataFimLista.Date.AddDays(1); 
                        }

                        if (DateTime.Now.Date < dataInicioLista.Date.AddDays(congregacao.DiasAntecedenciaGerarLista * -1))
                        {
                            model.AddNotification("Congregacao", $"A lista somente pode ser gerada a partir de + " +
                                $"'{dataInicioLista.Date.AddDays(congregacao.DiasAntecedenciaGerarLista * -1).Date}'");
                            unitOfWork.Rollback();
                            return model.Notifications;
                        }

                        var tipolistas = _repositoryTipoLista.ListAll(ref unitOfWork, congregacao.Codigo).ToList();
                        int quadro = 0;
                        int quantidadeDatas = 0;
                        if (tipolistas.Count() > 0)
                        {
                            quadro = _repositoryQuadro.ObterCodigoProximoQuadro(ref unitOfWork);
                            // Incluir / retirar irmãos da lista / atualiza designações
                            AtualizarControleLista(ref unitOfWork, congregacao.Codigo, true);
                            quantidadeDatas = tipolistas.FirstOrDefault().QuantidadeDatas;

                            for (int iv = 0; iv < tipolistas.Count(); iv++)
                            {
                              tipolistas[iv].CodigoQuadro = _repositoryQuadro.InserirNovoQuadro(ref unitOfWork, congregacao.Codigo, quadro, (int)tipolistas[iv].Codigo);
                            }
                        }

                        int i = 0;
                        DateTime dataControle = dataInicioLista;
                        while (i < quantidadeDatas)
                        {
                        bool assembleia = false;
                        bool dataReuniaoValida = false;
                         foreach (var itemTipoLista in tipolistas)
                            {
                            switch (itemTipoLista.Codigo)
                            {
                                case Core.Domain.enums.eTipoLista.IndicadorEntrada:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        for (int iIndicador = 0; iIndicador < congregacao.QuantidadeIndicadores; iIndicador++)
                                        {
                                            if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        }
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.IndicadorAuditorio:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.AudioVideo:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        for (int iSistemaSonoro = 0; iSistemaSonoro < congregacao.QuantidadeSistemaSonoro; iSistemaSonoro++)
                                        {
                                            if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                            dataReuniaoValida = true;
                                        }
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.Microfonista:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        for (int iMicrofonistas = 0; iMicrofonistas < congregacao.QuantidadeMicrofonistas; iMicrofonistas++)
                                        {
                                            if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        }
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.OracaoFinal:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.LeitorJW:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela)
                                    {
                                        InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                case Core.Domain.enums.eTipoLista.LeitorELC:
                                    if (dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                                    {
                                        InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                        dataReuniaoValida = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        dataControle = dataControle.AddDays(1);
                        if (dataReuniaoValida) i++;
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

        public List<GetQuadroDesignacaoMecanica> ObterListaDesignacoesMecanicas(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var quadroAtual = _repositoryQuadro.ObterQuadroAtual(ref unitOfWork, congregacaoId);
                    var proximoQuadro = _repositoryQuadro.ObterProximoQuadro(ref unitOfWork, congregacaoId);

                    return ObterListaPorQuadroId(ref unitOfWork, quadroAtual, proximoQuadro);
                }
                catch
                {
                    throw;
                }
            }
        }

        public IReadOnlyCollection<Notification> RegerarListaAtual(int congregacaoAtual)
        {
            var model = new Quadro();
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var congregacao = _repositoryCongregacao.ListAll(ref unitOfWork).ToList().Find(x => x.Codigo == congregacaoAtual);

                    if (congregacao == null)
                    {
                        model.AddNotification("Congregacao", "Não existe nenhuma congregação cadastrada");
                        unitOfWork.Rollback();
                        return model.Notifications;
                    }

                    var quadroAtual = _repositoryQuadro.ObterQuadroAtual(ref unitOfWork, congregacaoAtual);
                    var proximoQuadro = _repositoryQuadro.ObterProximoQuadro(ref unitOfWork, congregacaoAtual);
                    int quadro = 0;
                    DateTime dataInicioLista;

                    if (proximoQuadro > 0)
                        quadro = proximoQuadro;
                    else
                        quadro = quadroAtual;

                    dataInicioLista = _repositoryQuadroDetalhe.ObterDataInicioQuadro(ref unitOfWork, quadro);

                    var tipolistas = _repositoryTipoLista.ListAll(ref unitOfWork, congregacaoAtual).ToList();
                    DateTime dataFinalLista = _repositoryQuadroDetalhe.ObterDataFimQuadro(ref unitOfWork, quadro);
                    //Apaga Lista Atual
                    _repositoryQuadroDetalhe.ApagaDetalhesQuadro(ref unitOfWork, quadro);

                    // Gera lista até data atual
                    GeraListas(ref unitOfWork, ref tipolistas, ref congregacao, dataInicioLista, quadro, dataFinalLista, true);

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

        private void GeraListas(ref IUnitOfWork unitOfWork, ref List<TipoLista> tipolistas, ref Congregacao congregacao, DateTime dataInicioLista, int quadro, DateTime datalimite, 
            bool regerarlista)
        {
            for (int iv = 0; iv < tipolistas.Count(); iv++)
            {
                if (regerarlista)
                {
                    tipolistas[iv].CodigoQuadro = _repositoryQuadro.InserirNovoQuadro(ref unitOfWork, congregacao.Codigo, quadro, (int)tipolistas[iv].Codigo);
                }
                else
                    tipolistas[iv].CodigoQuadro = _repositoryQuadro.ObterQuadroTipoLista(ref unitOfWork, quadro, (int)tipolistas[iv].Codigo);
            }

            var quantidadeDatas = tipolistas.FirstOrDefault().QuantidadeDatas;

            int i = 0;
            DateTime dataControle = dataInicioLista;
            while (i < quantidadeDatas)
            {
                bool assembleia = false;
                bool dataReuniaoValida = false;

                foreach (var itemTipoLista in tipolistas)
                {
                    switch (itemTipoLista.Codigo)
                    {
                        case Core.Domain.enums.eTipoLista.IndicadorEntrada:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                for (int iIndicador = 0; iIndicador < congregacao.QuantidadeIndicadores; iIndicador++)
                                {
                                    if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                }
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.IndicadorAuditorio:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.AudioVideo:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                for (int iSistemaSonoro = 0; iSistemaSonoro < congregacao.QuantidadeSistemaSonoro; iSistemaSonoro++)
                                {
                                    if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                }
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.Microfonista:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                for (int iMicrofonistas = 0; iMicrofonistas < congregacao.QuantidadeMicrofonistas; iMicrofonistas++)
                                {
                                    if (!assembleia) assembleia = InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                }
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.OracaoFinal:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela || dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.LeitorJW:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoSentinela)
                            {
                                InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                dataReuniaoValida = true;
                            }
                            break;
                        case Core.Domain.enums.eTipoLista.LeitorELC:
                            if (dataControle.DayOfWeek == congregacao.DiaReuniaoServico)
                            {
                                if (dataControle.Date <= datalimite.Date) InsereDetalheQuadro(ref unitOfWork, dataControle, congregacao, itemTipoLista.CodigoQuadro, itemTipoLista);
                                dataReuniaoValida = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                dataControle = dataControle.AddDays(1);
                // Para a atualização
                if (dataControle.Date > datalimite.Date)
                    i = quantidadeDatas;

                if (dataReuniaoValida) i++;
            }
        }

        private List<GetQuadroDesignacaoMecanica> ObterListaPorQuadroId(ref IUnitOfWork unitOfWork, int quadroAtual, int proximoQuadro)
        {
            var dias = (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || DateTime.Now.DayOfWeek == DayOfWeek.Friday) ? ((int)DateTime.Now.DayOfWeek) : 5;
            var lista = _repositoryQuadro.ObterListaDesignacoesMecanicas(ref unitOfWork, quadroAtual, proximoQuadro, dias);
            for (int i = 0; i < lista.Count; i++)
            {
                lista[i].DataFormatada = lista[i].Data.ToString("dd/MM");

                var indicadores = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.IndicadorEntrada, quadroAtual, proximoQuadro, lista[i].Data);
                lista[i].Indicadores = new List<string>();
                foreach (var item in indicadores)
                {
                    lista[i].Indicadores.Add(item.Nome);
                }

                var microfonistas = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.Microfonista, quadroAtual, proximoQuadro, lista[i].Data);
                lista[i].Microfonistas = new List<string>();
                foreach (var item2 in microfonistas)
                {
                    lista[i].Microfonistas.Add(item2.Nome);
                }

                var somvideo = _repositoryQuadroDetalhe.ObterIrmaosTipoLista(ref unitOfWork, Core.Domain.enums.eTipoLista.AudioVideo, quadroAtual, proximoQuadro, lista[i].Data);
                lista[i].SomVideo = new List<string>();
                foreach (var item3 in somvideo)
                {
                        lista[i].SomVideo.Add(item3.Nome);
                }
            }
            return lista;
        }

        private void AtualizarControleLista(ref IUnitOfWork unitOfWork, int congregacaoId, bool atualizaFlagAtualizarDesignacoes)
        {
            var irmaos = _repositoryIrmao.ObterIrmaosADesativarOuAtivar(ref unitOfWork, congregacaoId);
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
                        ControleLista lista = _repositoryControleLista.GetListaIrmao(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                        switch (item.Codigo)
                        {
                            case Core.Domain.enums.eTipoLista.IndicadorEntrada:
                                if (itemIrmao.Indicador && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.Indicador && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.IndicadorAuditorio:
                                if (itemIrmao.IndicadorAuditorio && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.IndicadorAuditorio && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.Microfonista:
                                if (itemIrmao.Microfonista && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.Microfonista && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.LeitorJW:
                                if (itemIrmao.LeitorSentinela && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.LeitorSentinela && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.OracaoFinal:
                                if (itemIrmao.OracaoFinal && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.OracaoFinal && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.LeitorELC:
                                if (itemIrmao.LeitorEstudoLivro && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.LeitorEstudoLivro && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            case Core.Domain.enums.eTipoLista.AudioVideo:
                                if (itemIrmao.SistemaSonoro && lista == null)
                                {
                                    _repositoryControleLista.IncluirIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo, inicioLista);
                                    inicioLista = !inicioLista;
                                }
                                else if (!itemIrmao.SistemaSonoro && lista != null)
                                    _repositoryControleLista.RemoverIrmaoLista(ref unitOfWork, (int)item.Codigo, itemIrmao.Codigo);
                                break;
                            default:
                                break;
                        }
                    }
                    // Caso seja geração de uma nova lista, desabilitar flag que atualiza a lista
                    if (atualizaFlagAtualizarDesignacoes) _repositoryIrmao.AtualizaFlagDesignacao(ref unitOfWork, itemIrmao.Codigo);
                }
            }
        }

        private bool InsereDetalheQuadro(ref IUnitOfWork unitOfWork, DateTime dataControle, Congregacao item, int codigoQuadro, TipoLista itemTipoLista)
        {
            var cont = 0;
            ControleLista proximoLista = null;
            bool liberouproximo = false;
            DataEvento evento = _repositoryDataEvento.ListByDate(ref unitOfWork, item.Codigo, dataControle);

            if (itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.LeitorJW || itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.LeitorELC)
            {
                Trace.WriteLine("Lista Microfose");
            }

            if (itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.IndicadorEntrada || itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.Microfonista)
            {
                Trace.WriteLine("Lista Microfose");
            }

            QuadroDetalhe quadrodetalhe;
            if (evento == null || (evento.VisitaSuperintendente && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorELC
                && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorJW))
            {
                if (itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.OracaoFinal && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorELC && itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.LeitorJW &&
                    itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.IndicadorAuditorio)
                {
                    while (proximoLista == null)
                    {
                        if (cont <= 1)
                            proximoLista =  _repositoryControleLista.ObterMaisAntigoGeralSemRepetir(ref unitOfWork,
                              (int)itemTipoLista.Codigo, dataControle, item.Codigo, itemTipoLista.Codigo.GetDescription());
                        else
                            throw new Exception("Não foi possivel obter um irmão da lista ");

                        cont++;
                        Task.Delay(10).Wait();
                    }
                }
                else if (itemTipoLista.Codigo != Core.Domain.enums.eTipoLista.OracaoFinal &&
                    (itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.LeitorELC || itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.LeitorJW || itemTipoLista.Codigo == Core.Domain.enums.eTipoLista.IndicadorAuditorio))
                {
                    while (proximoLista == null)
                    {
                        if (cont <= 1)
                            proximoLista = _repositoryControleLista.ObterMaisAntigoDesignacaoLeitorSemRepetir(ref unitOfWork, (int)itemTipoLista.Codigo,
                              dataControle, item.Codigo, itemTipoLista.Codigo.GetDescription());
                        else
                            throw new Exception("Não foi possivel obter um irmão da lista de Leitor");

                        cont++;
                    }
                }
                else
                {
                    while (proximoLista == null)
                    {
                        proximoLista = _repositoryControleLista.ObterMaisAntigoDesignacaoLeitorSemRepetir(ref unitOfWork, (int)itemTipoLista.Codigo,
                              dataControle, item.Codigo, itemTipoLista.Codigo.GetDescription());
                    }
                }

                quadrodetalhe = new QuadroDetalhe() { Data = dataControle, IrmaoId = proximoLista.IrmaoId, QuadroId = codigoQuadro };
                _repositoryControleLista.AtualizaPartipacaoIrmaoLista(ref unitOfWork, (int)itemTipoLista.Codigo, proximoLista.IrmaoId, liberouproximo);
            }
            else
                quadrodetalhe = new QuadroDetalhe() { Data = dataControle, QuadroId = codigoQuadro, EventoId = evento.Codigo };

            _repositoryQuadroDetalhe.InsereDataQuadro(ref unitOfWork, quadrodetalhe);

            return (evento != null && evento.Assembleia);
        }
    }
}
