using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class ExcecaoDesignacaoRepository : IExcecaoDesignacaoRepository
    {
        public void Apagar(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute(@" DELETE FROM [dbo].[ExcecaoDesignacao]
                                             WHERE Codigo = @Id",
                                        param: new
                                        {
                                            @Id = id
                                        },
                                        transaction: unitOfWork.Transaction);
        }

        public bool ExisteExcecaoDesignacaoPorIrmaoEData(ref IUnitOfWork unitOfWork, int irmaoId, DateTime data)
        {
            var ret = Convert.ToBoolean(unitOfWork.Connection.ExecuteScalar("p_sel_ExisteExcecaoDesignacao",
                            param: new
                            {
                                @IrmaoId = irmaoId,
                                @Data = data.ToString("yyyy-MM-dd")
                            },
                            commandType: CommandType.StoredProcedure,
                           transaction: unitOfWork.Transaction
                           ));

            return ret;
        }

        public ExcecaoDesignacao FindById(ref IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.Connection.Query<ExcecaoDesignacao>("p_sel_ExcecaoDesignacaoPorId",
                            param: new
                            {
                                @Id = id
                            },
                            commandType: CommandType.StoredProcedure,
                            transaction: unitOfWork.Transaction
                           ).FirstOrDefault();
        }

        public void Inserir(ref IUnitOfWork unitOfWork, ExcecaoDesignacao model)
        {
            unitOfWork.Connection.Execute("p_ins_ExcecaoDesignacao",
                            param: new
                            {
                                model.IrmaoId,
                                model.Data,
                                model.Motivo
                            },
                            commandType: CommandType.StoredProcedure,
                           transaction: unitOfWork.Transaction
                           );
        }

        public IEnumerable<ExcecaoDesignacao> ObterExcecaoPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<ExcecaoDesignacao>("p_sel_ExcecaoDesignacao",
                            param: new
                            {
                                @CongregacaoId = congregacaoId
                            },
                            commandType: CommandType.StoredProcedure
                           ).ToList();

        }
    }
}
