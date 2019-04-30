using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IIrmaoRepository
    {
        Irmao FindById(ref IUnitOfWork unitOfWork, int id);
        void AtualizaAtivarProximaLista(ref IUnitOfWork unitOfWork, int id);
        void AtualizaDesativarProximaLista(ref IUnitOfWork unitOfWork, int id);
        IEnumerable<Irmao> ObterIrmaosADesativarOuAtivar(ref IUnitOfWork unitOfWork, int congregacaoId);
        IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
