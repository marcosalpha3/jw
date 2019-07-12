using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IQuadroRepository
    {
        int ObterCodigoProximoQuadro(ref IUnitOfWork unitOfWork);
        int InserirNovoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId, int quadroId, int tipoListaId);
        List<GetQuadroDesignacaoMecanica> ObterListaDesignacoesMecanicas(ref IUnitOfWork unitOfWork, int QuadroId);
        Quadro ObterUltimoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId);
        int ObterQuadroAtual(ref IUnitOfWork unitOfWork, int congregacaoId);
        int ObterProximoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId);
        void InserirQuadroPersonalizado(ref IUnitOfWork unitOfWork, QuadroPersonalizado model);
        void AlterarStatusStorageQuadroPersonalizado(ref IUnitOfWork unitOfWork, string url);
        List<QuadroPersonalizado> ObterQuadrosPersonalizadosExpiradosAtivosStorage(ref IUnitOfWork unitOfWork);
        List<QuadroPersonalizado> ObterQuadrosPersonalizadosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
        void ApagarQuadroPersonalizado(ref IUnitOfWork unitOfWork, string url, int congregacaoId);
        QuadroPersonalizado ObterQuadroPersonalizado(ref IUnitOfWork unitOfWork, int congregacaoId, string url);
        List<QuadroPersonalizado> ObterQuadrosPersonalizadosAtivosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
