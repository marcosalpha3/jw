using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface ITipoListaRepository
    {
        IEnumerable<TipoLista> ListAll(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
