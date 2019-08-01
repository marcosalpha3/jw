using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practice.Common.Values;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class DataEventoRepository : IDataEventoRepository
    {
        public void Inserir(ref IUnitOfWork unitOfWork, DataEvento model)
        {
            unitOfWork.Connection.Execute(@" INSERT INTO [dbo].[DataEvento]
                                             ([Data], [Descricao], [CongregacaoId], [Assembleia], [VisitaSuperintendente])
                                             VALUES
                                             (@Data, @Descricao, @CongregacaoId, @Assembleia, @VisitaSuperintendente) ",
                               param: new
                               {
                                   @Data = model.Data,
                                   @Descricao = model.Descricao,
                                   @CongregacaoId = model.CongregacaoId,
                                   @Assembleia = model.Assembleia,
                                   @VisitaSuperintendente = model.VisitaSuperintendente
                               },
                               transaction: unitOfWork.Transaction);
        }

        public void Apagar(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute(@" DELETE FROM [dbo].[DataEvento]
                                             WHERE Codigo = @Id",
                                        param: new
                                        {
                                            @Id = id
                                        },
                                        transaction: unitOfWork.Transaction);
        }

        public DataEvento FindById(ref IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.Connection.Query<DataEvento>("Select * from [dbo].[DataEvento] where Codigo = @id",
                    param: new
                    {
                        @id = id
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public IEnumerable<DataEvento> ObterEventosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<DataEvento>("Select * from [dbo].[DataEvento] where CongregacaoId = @congregacao and Data >= CAST(DATEADD(DAY, -1, getdate()) As Date) order by Data ",
                param: new
                {
                    @congregacao = congregacaoId
                }
               );
        }

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

        public DataEvento PesquisarporDataeCongregacao(ref IUnitOfWork unitOfWork, DataEvento model, int? id = null)
        {
            return unitOfWork.Connection.Query<DataEvento>(
                      (id != null) ? @"SELECT DE.* FROM dbo.[DataEvento] DE 
                                     WHERE Data = CAST(@Data As Date) and CongregacaoId = @CongregacaoId  and DE.Codigo <> @Codigo " :
                                     @" SELECT DE.* FROM 
                                        dbo.[DataEvento] DE 
                                        WHERE Data = CAST(@Data As Date) and CongregacaoId = @CongregacaoId ",
                      param: new
                      {
                          @Data = model.Data,
                          @CongregacaoId = model.CongregacaoId,
                          @Codigo = (IntValues.IsNullorDefault(id)) ? (int?)null : id
                      },
                      transaction: unitOfWork.Transaction
                  ).FirstOrDefault();
        }


    }
}
