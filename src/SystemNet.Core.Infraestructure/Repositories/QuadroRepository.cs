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
                                                           AND QDL.Data >= CAST(DATEADD(DAY, @Dias, getdate()) As Date)
                                                           AND (Q.Quadro = @QuadroAtual or Q.Quadro = @QuadroProximo)
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

        public void InserirQuadroPersonalizado(ref IUnitOfWork unitOfWork, QuadroPersonalizado model)
        {
            unitOfWork.Connection.Execute(@" INSERT INTO [dbo].[QuadroPersonalizado]
                                                          ([Url], [Titulo], [CongregacaoId], [DataExpiracao], [DataInicio], [AtivoStorage])
                                                          VALUES
                                                         (@Url, @Titulo, @CongregacaoId, @DataExpiracao, @DataInicio, @AtivoStorage)",
               param: new
               {
                   @Url = model.Url,
                   @Titulo = model.Titulo,
                   @CongregacaoId = model.CongregacaoId,
                   @DataExpiracao = model.DataExpiracao,
                   @DataInicio = model.DataInicio,
                   @AtivoStorage = model.AtivoStorage
               }, transaction: unitOfWork.Transaction
               );
        }

        public void AlterarStatusStorageQuadroPersonalizado(ref IUnitOfWork unitOfWork, string url)
        {
            unitOfWork.Connection.Execute(@" UPDATE QuadroPersonalizado set AtivoStorage = 0 where Url = @Url",
               param: new
               {
                   @Url = url
               }, transaction: unitOfWork.Transaction
               );
        }

        public void ApagarQuadroPersonalizado(ref IUnitOfWork unitOfWork, string url, int congregacaoId)
        {
            unitOfWork.Connection.Execute(@" DELETE FROM QuadroPersonalizado where Url = @Url and congregacaoId = @congregacaoId",
               param: new
               {
                   @Url = url,
                   @congregacaoId = congregacaoId
               }, transaction: unitOfWork.Transaction
               );
        }

        public List<QuadroPersonalizado> ObterQuadrosPersonalizadosAtivosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<QuadroPersonalizado>(@" SELECT * FROM QuadroPersonalizado WHERE CongregacaoId = @CongregacaoId AND DataInicio <= CAST(GETDATE() AS DATE) and 
                                                                       DataExpiracao >= CAST(GETDATE() AS DATE) ",
                    param: new
                    {
                        @CongregacaoId = congregacaoId
                    }
                ).ToList();
        }

        public List<QuadroPersonalizado> ObterQuadrosPersonalizadosExpiradosAtivosStorage(ref IUnitOfWork unitOfWork)
        {
            return unitOfWork.Connection.Query<QuadroPersonalizado>(@" Select * from QuadroPersonalizado where DataExpiracao < GetDate() and AtivoStorage = 1 ",
                transaction: unitOfWork.Transaction
                ).ToList();
        }

        public List<QuadroPersonalizado> ObterQuadrosPersonalizadosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<QuadroPersonalizado>(@" Select * from QuadroPersonalizado where DataExpiracao >= GetDate() and AtivoStorage = 1 
                                                                               and CongregacaoId = @CongregacaoId ",
                    param: new
                    {
                        @CongregacaoId = congregacaoId
                    }
                ).ToList();
        }

        public QuadroPersonalizado ObterQuadroPersonalizado(ref IUnitOfWork unitOfWork, int congregacaoId, string url)
        {
            return unitOfWork.Connection.Query<QuadroPersonalizado>(@" Select * from QuadroPersonalizado where Url = @Url and congregacaoId = @congregacaoId ",
                param: new
                {
                    @CongregacaoId = congregacaoId,
                    @Url = url
                },
                   transaction: unitOfWork.Transaction
               ).FirstOrDefault();
        }


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

        public int ObterQuadroTipoLista(ref IUnitOfWork unitOfWork, int quadroId, int tipoListaId)
        {
            return Convert.ToInt32(unitOfWork.Connection.ExecuteScalar(@" select Codigo from Quadro where Quadro = @Quadro and TipoListaId = @TipoListaId ",
               param: new
               {
                   @Quadro = quadroId,
                   @TipoListaId = tipoListaId
               },
               transaction: unitOfWork.Transaction));
        }

        public int ObterCodigoProximoQuadro(ref IUnitOfWork unitOfWork)
        {
            return (int)unitOfWork.Connection.ExecuteScalar("SELECT ISNULL(Max(Quadro), 0) + 1 from dbo.Quadro",
            transaction: unitOfWork.Transaction);
        }

        public List<GetQuadroDesignacaoMecanica> ObterListaDesignacoesMecanicas(ref IUnitOfWork unitOfWork, int quadroAtual, int quadroProximo, int dias)
        {
            return unitOfWork.Connection.Query<GetQuadroDesignacaoMecanica>(SelectDesignacoesMecanicas,
                    param: new { @TipoLeitorELC = eTipoLista.LeitorELC,
                                 @TipoLeitorJW = eTipoLista.LeitorJW,
                                 @TipoOracaoFinal = eTipoLista.OracaoFinal,
                                 @QuadroAtual = quadroAtual,
                                 @QuadroProximo = quadroProximo,
                                 @Dias = dias
                    }
                ).ToList();
        }

        public int ObterProximoQuadro(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var quadro = unitOfWork.Connection.ExecuteScalar(SelectProximoQuadro,
               param: new
               {
                   @CongregacaoId = congregacaoId,
               },
                   transaction: unitOfWork.Transaction
               );

            return (quadro == null) ? 0 : (int)quadro;
        }

        public int ObterQuadroAtual(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var quadro = unitOfWork.Connection.ExecuteScalar(SelectQuadroAtual,
               param: new
               {
                   @CongregacaoId = congregacaoId,
               },
                   transaction: unitOfWork.Transaction
               );

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
