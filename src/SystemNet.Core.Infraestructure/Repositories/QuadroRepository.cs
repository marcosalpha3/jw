using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.enums;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class QuadroRepository : IQuadroRepository
    {
        #region [Scripts SQL]

        private const string insereQuadro = @" INSERT INTO [dbo].[Quadro] ([Quadro], [CongregacaoId], [TipoListaId])
                                               VALUES (@Quadro, @CongregacaoId, @TipoListaId) ; SELECT SCOPE_IDENTITY()";

        private const string SelectDesignacoesMecanicas = @"SELECT QDL.Data As Data, IL.Nome As Leitor,  DE.Descricao  Evento,
                                                           (SELECT top 1 IL2.Nome from dbo.Quadro Q2											
                                                           LEFT JOIN QuadroDetalhe QDL2 ON QDL2.QuadroId = Q2.Codigo 
                                                           LEFT JOIN Irmao IL2 ON IL2.Codigo = QDL2.IrmaoId
                                                           WHERE Q2.TipoListaId = @TipoOracaoFinal and QDL2.Data = QDL.DATA) As OracaoFinal
                                                           from dbo.Quadro Q											
                                                           LEFT JOIN QuadroDetalhe QDL ON QDL.QuadroId = Q.Codigo 
                                                           LEFT JOIN Irmao IL ON IL.Codigo = QDL.IrmaoId
                                                           LEFT JOIN DataEvento DE ON DE.Codigo = QDL.EventoId
                                                           WHERE (TipoListaId = @TipoLeitorELC OR TipoListaId = @TipoLeitorJW)
                                                           AND Q.Quadro = @Quadro
                                                           DATA >= GETDATE()
                                                           ORDER BY DATA ";

        private const string SelectUltimoQuadro = @"SELECT Tab.*, Min(QD.Data) As DataInicioLista, Max(QD.Data) As DataFimLista from
                                                    (SELECT top 1 Codigo, Quadro As QuadroId  FROM [dbo].[Quadro] Q
                                                    WHERE Q.CongregacaoId = @CongregacaoId
                                                    order by Codigo desc) As Tab
                                                    INNER JOIN QuadroDetalhe QD ON Tab.Codigo = QD.QuadroId
                                                    GROUP BY Tab.Codigo, Tab.QuadroId";

        private const string SelectQuadroAtual = @" SELECT TOP 1 QuadroId FROM 
                                                    (SELECT Quadro As QuadroId, Min(QD.Data) As DataMin, Max(QD.Data) As DataMax  FROM [dbo].[Quadro] Q
										            INNER JOIN QuadroDetalhe QD ON Q.Codigo = QD.QuadroId
                                                    WHERE Q.CongregacaoId = @congregacaoId
                                                    group By Quadro) As Tab
                                                    WHERE DataMin <= Cast(GETDATE() As Date)
                                                    ORDER BY QuadroId DESC ";

        private const string SelectProximoQuadro = @" SELECT TOP 1 QuadroId FROM 
                                                     (SELECT Quadro As QuadroId, Min(QD.Data) As DataMin, Max(QD.Data) As DataMax  FROM [dbo].[Quadro] Q
										             INNER JOIN QuadroDetalhe QD ON Q.Codigo = QD.QuadroId
                                                     WHERE Q.CongregacaoId = @congregacaoId
                                                     group By Quadro) As Tab
                                                     WHERE DataMin > Cast(GETDATE() As Date)
                                                     ORDER BY QuadroId desc ";

        #endregion

        public int InserirNovoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId, int quadroId, int tipoListaId)
        {
            return Convert.ToInt32(unitOfWork.Connection.ExecuteScalar(insereQuadro,
               param: new
               {
                   @Quadro = quadroId,
                   @CongregacaoId = congregacaoId,
                   @TipoListaId = tipoListaId
               },
               transaction: unitOfWork.Transaction));
        }

        public int ObterCodigoProximoQuadro(ref IUnitOfWork unitOfWork)
        {
            return (int)unitOfWork.Connection.ExecuteScalar("SELECT ISNULL(Max(Codigo), 0) + 1 from dbo.Quadro",
            transaction: unitOfWork.Transaction);
        }

        public List<GetQuadroDesignacaoMecanica> ObterListaDesignacoesMecanicas(ref IUnitOfWork unitOfWork, int QuadroId)
        {
            return unitOfWork.Connection.Query<GetQuadroDesignacaoMecanica>(SelectDesignacoesMecanicas,
                    param: new { @TipoLeitorELC = eTipoLista.LeitorELC,
                                 @TipoLeitorJW = eTipoLista.LeitorJW,
                                 @TipoOracaoFinal = eTipoLista.OracaoFinal,
                                 @Quadro = QuadroId
                    }
                ).ToList();
        }

        public int ObterProximoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var quadro = unitOfWork.Connection.ExecuteScalar(SelectProximoQuadro,
               param: new
               {
                   @CongregacaoId = congregacaoId,
               });

            return (quadro == null) ? 0 : (int)quadro;
        }

        public int ObterQuadroAtual(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var quadro = unitOfWork.Connection.ExecuteScalar(SelectQuadroAtual,
               param: new
               {
                   @CongregacaoId = congregacaoId,
               });

            return (quadro == null) ? 0 : (int)quadro;
        }

        public Quadro ObterUltimoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<Quadro>(SelectUltimoQuadro,
                param: new
                {
                    @CongregacaoId = congregacaoId
                },
                   transaction: unitOfWork.Transaction
               ).FirstOrDefault();
        }
    }
}
