using Dapper;
using System;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class DataEventoRepository : IDataEventoRepository
    {
        public DataEvento ListByDate(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data)
        {
            return unitOfWork.Connection.Query<DataEvento>(@" SELECT *  FROM [dbo].[DataEvento] where Data = Cast(@Data As Date) and 
                                                              CongregacaoId = @CongregacaoId",
                    param: new { @Data = data,
                                 @CongregacaoId = congregacaoId
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }
    }
}
