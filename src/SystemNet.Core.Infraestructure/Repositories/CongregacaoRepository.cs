using Dapper;
using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class CongregacaoRepository : ICongregacaoRepository
    {
        public IEnumerable<Congregacao> ListAll(ref IUnitOfWork unitOfWork)
        {
            return unitOfWork.Connection.Query<Congregacao>(
                " SELECT * FROM [dbo].[Congregacao] ",
                transaction : unitOfWork.Transaction
            );
        }
    }
}
