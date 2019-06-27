using Dapper;
using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class GrupoRepository : IGrupoRepository
    {
        public IEnumerable<Grupo> ObterGruposPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<Grupo>("Select * from dbo.Grupo where CongregacaoId = @congregacao order by Nome",
                param: new
                {
                    @congregacao = congregacaoId
                }
               );
        }
    }
}
