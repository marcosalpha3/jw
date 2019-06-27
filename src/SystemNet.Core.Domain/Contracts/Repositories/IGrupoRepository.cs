using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IGrupoRepository
    {
        IEnumerable<Grupo> ObterGruposPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
