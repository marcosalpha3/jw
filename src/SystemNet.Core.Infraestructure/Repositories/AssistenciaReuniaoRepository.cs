using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys.Assistencias;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class AssistenciaReuniaoRepository : IAssistenciaReuniaoRepository
    {
        public void Apagar(ref IUnitOfWork unitOfWork, DateTime data, int congregacaoId)
        {
            unitOfWork.Connection.Execute(@" DELETE FROM [dbo].[AssistenciasReunioes]
                                             WHERE CongregacaoId = @CongregacaoId and [Data] = @Data",
               param: new
               {
                   @CongregacaoId = congregacaoId,
                   @Data = data
               }, transaction: unitOfWork.Transaction
               );

        }

        public void Atualizar(ref IUnitOfWork unitOfWork, AssistenciaReuniao model)
        {
            unitOfWork.Connection.Execute(@" UPDATE [dbo].[AssistenciasReunioes]
                                             SET [AssistenciaParte1] = @AssistenciaParte1 ,[AssistenciaParte2] = @AssistenciaParte2
                                             WHERE CongregacaoId = @CongregacaoId and [Data] = @Data",
               param: new
               {
                   @CongregacaoId = model.CongregacaoId,
                   @AssistenciaParte1 = model.AssistenciaParte1,
                   @AssistenciaParte2 = model.AssistenciaParte2,
                   @Data = model.Data
               }, transaction: unitOfWork.Transaction
               );
        }

        public AssistenciaReuniao FindByData(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data)
        {
            return unitOfWork.Connection.Query<AssistenciaReuniao>(@" Select * from [dbo].[AssistenciasReunioes] where CongregacaoId = @CongregacaoId and [Data] = @Data ",
                            param: new
                            {
                                @CongregacaoId = congregacaoId,
                                @Data = data
                            },
                               transaction: unitOfWork.Transaction
                           ).FirstOrDefault();
        }

        public void Inserir(ref IUnitOfWork unitOfWork, AssistenciaReuniao model)
        {
            unitOfWork.Connection.Execute(@" INSERT INTO [dbo].[AssistenciasReunioes]
                                             ([Data], [CongregacaoId], [AssistenciaParte1], [AssistenciaParte2])
                                             VALUES
                                            (@Data, @CongregacaoId, @AssistenciaParte1,@AssistenciaParte2)",
               param: new
               {
                   @CongregacaoId = model.CongregacaoId,
                   @AssistenciaParte1 = model.AssistenciaParte1,
                   @AssistenciaParte2 = model.AssistenciaParte2,
                   @Data = model.Data
               }, transaction: unitOfWork.Transaction
               );
        }

        public List<GetDetalheAssistencia> ObterAssistenciasPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime dataInicial, DateTime dataFinal)
        {
            return unitOfWork.Connection.Query<GetDetalheAssistencia>(@" SELECT  [Data], [AssistenciaParte1] ,[AssistenciaParte2]
                                                                    FROM [AssistenciasReunioes] A
                                                                    INNER JOIN Congregacao C ON C.Codigo = A.CongregacaoId
                                                                    Where C.Codigo = @CongregacaoId
                                                                    and A.Data >= CAST(@DataInicial As Date) and A.Data <= CAST(@DataFinal As Date)
                                                                    ORDER BY Data desc",
                            param: new
                            {
                                @CongregacaoId = congregacaoId,
                                @DataInicial = dataInicial,
                                @DataFinal = dataFinal
                            }
                           ).ToList();

        }
    }
}
