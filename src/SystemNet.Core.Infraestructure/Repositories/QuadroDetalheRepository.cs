using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.enums;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class QuadroDetalheRepository : IQuadroDetalheRepository
    {
        #region [Scripts SQL]

        private const string SelectIrmaoDetalheLista = @"SELECT I.Nome, I.CongregacaoId FROM [dbo].[QuadroDetalhe] QD
                                                            INNER JOIN dbo.Quadro Q ON Q.Codigo = QD.QuadroId
															INNER JOIN dbo.Irmao I ON I.Codigo = QD.IrmaoId
                                                            Where (Q.Quadro = @QuadroAtual or Q.Quadro = @QuadroProximo)
															and QD.Data = CAST(@Data As Date)
                                                            and Q.TipoListaId = @TipoListaId ";

        private const string SelectUltimaReuniao = @" SELECT top 1 * FROM [dbo].[QuadroDetalhe] QD
                                                      INNER JOIN dbo.Quadro Q ON Q.Codigo = QD.QuadroId
                                                      Where Data < Cast(@data As Date)
                                                      and Q.CongregacaoId = @CongregacaoId 
                                                      order by Data desc";


        private const string SelectProximaReuniao = @"SELECT top 1 * FROM [dbo].[QuadroDetalhe] QD
                                                      INNER JOIN dbo.Quadro Q ON Q.Codigo = QD.QuadroId
                                                      Where Data > Cast(@data As Date)
                                                      and Q.CongregacaoId = @CongregacaoId
                                                      order by Data ";
        #endregion

        public void ApagaDetalhesQuadro(ref IUnitOfWork unitOfWork, int QuadroId)
        {
            unitOfWork.Connection.Execute("DELETE FROM [dbo].[Quadro] WHERE Quadro = @QuadroId",
                param: new
                {
                    @QuadroId = QuadroId
                },
                transaction: unitOfWork.Transaction);
        }

        public void InsereDataQuadro(ref IUnitOfWork unitOfWork, QuadroDetalhe model)
        {
            unitOfWork.Connection.Execute(@" INSERT INTO [dbo].[QuadroDetalhe] ([QuadroId], [Data], [IrmaoId], [EventoId])
                                             VALUES (@QuadroId, @Data, @IrmaoId, @EventoId) ",
                param: new
                {
                    @QuadroId = model.QuadroId,
                    @Data = model.Data,
                    @IrmaoId = (model.IrmaoId <= 0) ? (int?)null : model.IrmaoId,
                    @EventoId = (model.EventoId <= 0) ? (int?)null : model.EventoId,
                },
                transaction: unitOfWork.Transaction);
        }

        public List<Irmao> ObterIrmaosTipoLista(ref IUnitOfWork unitOfWork, eTipoLista tipolist, int quadroAtual, int quadroProximo, DateTime data)
        {
            return unitOfWork.Connection.Query<Irmao>(SelectIrmaoDetalheLista,
                    param: new
                    {
                        @QuadroAtual = quadroAtual,
                        @QuadroProximo = quadroProximo,
                        @TipoListaId = tipolist,
                        @Data = data
                    }
               ).ToList();
        }

        public QuadroDetalhe ObterUltimaReuniaoValida(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data)
        {
            return unitOfWork.Connection.Query<QuadroDetalhe>(SelectUltimaReuniao,
                param: new
                {
                    @data = data,
                    @CongregacaoId = congregacaoId
                },
                    transaction: unitOfWork.Transaction
                    ).FirstOrDefault();
        }

        public QuadroDetalhe ObterProximaReuniaoValida(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data)
        {
            return unitOfWork.Connection.Query<QuadroDetalhe>(SelectProximaReuniao,
                param: new
                {
                    @data = data,
                    @CongregacaoId = congregacaoId
                },
                    transaction: unitOfWork.Transaction
                    ).FirstOrDefault();
        }


        public DateTime ObterDataInicioQuadro(ref IUnitOfWork unitOfWork, int quadroId)
        {
            return Convert.ToDateTime(unitOfWork.Connection.ExecuteScalar(@" select Min(QD.Data) from Quadro Q
                                                          INNER JOIN QuadroDetalhe QD ON QD.QuadroId = Q.Codigo
                                                          WHERE Q.Quadro = @Quadro ",
                param: new
                {
                    @Quadro = quadroId
                },
                    transaction: unitOfWork.Transaction
                    ));
        }

        public DateTime ObterDataFimQuadro(ref IUnitOfWork unitOfWork, int quadroId)
        {
            return Convert.ToDateTime(unitOfWork.Connection.ExecuteScalar(@" select Max(QD.Data) from Quadro Q
                                                          INNER JOIN QuadroDetalhe QD ON QD.QuadroId = Q.Codigo
                                                          WHERE Q.Quadro = @Quadro ",
                param: new
                {
                    @Quadro = quadroId
                },
                    transaction: unitOfWork.Transaction
                    ));
        }

    }
}
