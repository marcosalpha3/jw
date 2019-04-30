using Dapper;
using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class TipoListaRepository : ITipoListaRepository
    {
        public IEnumerable<TipoLista> ListAll(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<TipoLista>("SELECT * FROM [dbo].[TipoLista] WHERE CongregacaoId = @CongregacaoId order by Descricao",
                param: new
                {
                    @CongregacaoId = congregacaoId
                },
                   transaction: unitOfWork.Transaction
               );
        }
    }
}
