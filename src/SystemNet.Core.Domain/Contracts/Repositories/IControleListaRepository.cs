using System;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IControleListaRepository
    {
        void BackupListaAtual(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime dataInicioLista, int congregacaoId);
        void RecuperaBackupListaAtual(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId);
        ControleLista ObterProximoListaSemRepetirComFolga(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datareuniaoanterior, DateTime datereuniaoAtual,
                    DateTime dataProximaReuniao, int congregacaoId);
        ControleLista ObterProximoListaSemRepetirSemFolga(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datereuniaoAtual, DateTime datareuniaoanterior,
                    DateTime dataProximaReuniao, int congregacaoId);
        ControleLista ObterProximoListaPodeRepetir(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId, DateTime dataReuniaoAtual);
        void AtualizaPartipacaoIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId, bool liberaproximo);
        void IncluirIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId, bool inicioLista);
        void RemoverIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId);
        ControleLista GetListaIrmao(ref IUnitOfWork unitOfWork, int tipoListaId, int irmao);
        void LiberaProximoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId);
        ControleLista ObterProximoListaSemRepetirSemFolgaParaAudioSonoro(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datareuniaoanterior, DateTime datereuniaoAtual,
                    DateTime dataProximaReuniao, int congregacaoId);

    }
}
