using Dapper;
using System;
using System.Linq;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.enums;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class ControleListaRepository : IControleListaRepository
    {
        #region [Scripts SQL]

        private const string backupListaAtualSQL = @" DELETE FROM dbo.BackupControleLista WHERE TipoListaId = @TipoListaId and CongregacaoId = @CongregacaoId;

                                                      INSERT INTO[dbo].[BackupControleLista]
                                                      (CodigoControleLista, [TipoListaId], [IrmaoId], CongregacaoId, [Participou], [DataInicioLista], OrdenaFinal, Proximo, Participacoes)
                                                      SELECT CodigoControleLista, TipoListaId, IrmaoId, @CongregacaoId, Participou, CAST(@DataInicioLista AS DATE), OrdenaFinal, Proximo, Participacoes 
                                                      from dbo.ControleLista WHERE TipoListaId = @TipoListaId and CongregacaoId = @CongregacaoId ";

        private const string recuperaBackupLista = @"DELETE FROM dbo.ControleLista WHERE TipoListaId = @TipoListaId and CongregacaoId = @CongregacaoId;

                                                     SET IDENTITY_INSERT dbo.ControleLista ON;  

                                                     INSERT INTO [dbo].[ControleLista]
                                                     (CodigoControleLista, [TipoListaId], [IrmaoId], [CongregacaoId], [Participou], OrdenaFinal, Proximo, Participacoes)
                                                     SELECT CodigoControleLista, [TipoListaId], [IrmaoId], [CongregacaoId], [Participou], OrdenaFinal, Proximo, Participacoes
                                                     from BackupControleLista WHERE TipoListaId = @TipoListaId and CongregacaoId = @CongregacaoId;

                                                     SET IDENTITY_INSERT dbo.ControleLista OFF;  ";

        private const string insertIrmaoLista = @" INSERT INTO [dbo].[ControleLista] ( [TipoListaId], [IrmaoId], [CongregacaoId] ,[Participou], [OrdenaFinal])
                                                    SELECT @TipoListaId, @IrmaoId, CongregacaoId, 0, @OrdenaFinal  FROM dbo.Irmao WHERE Codigo = @IrmaoId";

        private const string SelectProximoListaComFolga = @" SELECT top 1 * from ControleLista where TipoListaId = @TipoListaId and Participou = 0 

                                                           and CongregacaoId = @CongregacaoId

                                                           and IrmaoId NOT IN
                                                            (
                                                             SELECT EX.IrmaoId FROM ExcecaoDesignacao EX
                                                             INNER JOIN Irmao I3 on I3.Codigo = EX.IrmaoId
                                                             where EX.Data = CAST(@DataReuniaoAtual AS DATE)
                                                             and I3.CongregacaoId = @CongregacaoId
                                                            ) 

                                                             and IrmaoId NOT IN (select IrmaoId from
                                                            (select *, Case when SistemaSonoro = 1 and Hoje = 1 then 1 else 1 end As Repetir 
                                                            from
                                                            (select ISNULL(IrmaoId, 0) As IrmaoId, I2.SistemaSonoro,
															 (select 1  from dbo.QuadroDetalhe QD3
                                                             INNER JOIN Quadro Q3 ON Q3.Codigo = QD3.QuadroId
                                                             where Q3.TipoListaId <> @ListaOracao and
                                                             QD3.Data = CAST(@DataReuniaoAtual AS DATE)
															 AND QD3.IrmaoId = QD.IrmaoId) As Hoje
                                                             from dbo.QuadroDetalhe QD
                                                             INNER JOIN Quadro Q ON Q.Codigo = QD.QuadroId
															 INNER JOIN Irmao I2 ON QD.IrmaoId = I2.Codigo
                                                             where Q.TipoListaId <> @ListaOracao and
                                                            (Data = CAST(@DataReuniaoAnterior AS DATE) OR Data = 
                                                             CAST(@DataReuniaoAtual AS DATE) OR Data = CAST(@DataProximaReuniao As Date))) As TabNot) As TabNot2
                                                             WHERE Repetir = 1 
)
                                                             Order by  Participacoes, CodigoControleLista ";

        private const string SelectProximoListaSemFolgaAudio = @" SELECT top 1 * from ControleLista where TipoListaId = @TipoListaId and Participou = 0 

                                                               and CongregacaoId = @CongregacaoId

                                                               and IrmaoId NOT IN
                                                               (
                                                                SELECT EX.IrmaoId FROM ExcecaoDesignacao EX
                                                                INNER JOIN Irmao I3 on I3.Codigo = EX.IrmaoId
                                                                where EX.Data = CAST(@DataReuniaoAtual AS DATE)
                                                                and I3.CongregacaoId = @CongregacaoId
                                                               ) 


                                                             and IrmaoId NOT IN (select IrmaoId from
                                                            (select *, Case when SistemaSonoro = 1 and Hoje = 1 then 1 when SistemaSonoro = 1 and Hoje is null then 0 else 1 end As Repetir 
                                                            from
                                                            (select ISNULL(IrmaoId, 0) As IrmaoId, I2.SistemaSonoro,
															 (select 1  from dbo.QuadroDetalhe QD3
                                                             INNER JOIN Quadro Q3 ON Q3.Codigo = QD3.QuadroId
                                                             where Q3.TipoListaId <> @ListaOracao and
                                                             QD3.Data = CAST(@DataReuniaoAtual AS DATE)
															 AND QD3.IrmaoId = QD.IrmaoId) As Hoje
                                                             from dbo.QuadroDetalhe QD
                                                             INNER JOIN Quadro Q ON Q.Codigo = QD.QuadroId
															 INNER JOIN Irmao I2 ON QD.IrmaoId = I2.Codigo
                                                             where Q.TipoListaId <> @ListaOracao and
                                                            (Data = CAST(@DataReuniaoAnterior AS DATE) OR Data = 
                                                             CAST(@DataReuniaoAtual AS DATE) OR Data = CAST(@DataProximaReuniao As Date))) As TabNot) As TabNot2
                                                             WHERE Repetir = 1 
)
                                                             Order by Participacoes, CodigoControleLista ";


        private const string SelectProximoListaSemFolga = @" SELECT top 1 * from ControleLista where TipoListaId = @TipoListaId and Participou = 0 

                                                            and CongregacaoId = @CongregacaoId

                                                            and IrmaoId NOT IN
                                                            (
                                                             SELECT EX.IrmaoId FROM ExcecaoDesignacao EX
                                                             INNER JOIN Irmao I3 on I3.Codigo = EX.IrmaoId
                                                             where EX.Data = CAST(@DataReuniaoAtual AS DATE)
                                                             and I3.CongregacaoId = @CongregacaoId
                                                            ) 

                                                             and IrmaoId NOT IN ( select ISNULL(IrmaoId, 0) from dbo.QuadroDetalhe where Data = CAST(@DataReuniaoAtual AS DATE) )
                                                             AND IrmaoId NOT IN ( select ISNULL(IrmaoId, 0) from dbo.QuadroDetalhe QD
                                                             inner join Quadro Q ON Q.Codigo = QD.QuadroId
                                                             where Q.TipoListaId <> @TipoListaId and  (Data = CAST(@DataReuniaoAnterior AS DATE) 
                                                             OR Data = CAST(@DataReuniaoProxima AS DATE)))
                                                              Order by  Participacoes, CodigoControleLista  ";

        private const string LiberaProximo = @"DECLARE @CodigoControle int 
                                                    DECLARE @Indice int 
                                                    DECLARE @IndiceMax INT
                                                    DECLARE @IndiceMin INT
    
                                                    IF NOT EXISTS(SELECT 1 FROM [dbo].[ControleLista] WHERE Proximo = 0 and TipoListaId = @TipoListaId)
                                                     BEGIN
                                                      UPDATE [dbo].[ControleLista] SET Proximo = 0 where TipoListaId = @TipoListaId
                                                     END 

                                                    SELECT top 1 @CodigoControle = [CodigoControleLista]
                                                    FROM [QuadroJw].[dbo].[ControleLista]
                                                    where TipoListaId = @TipoListaId
                                                    and Participou = 0
                                                    and Proximo = 0
                                                    Order by OrdenaFinal, CodigoControleLista DESC

                                                    IF (@CodigoControle IS NULL)
                                                     BEGIN
                                                      SELECT top 1 @CodigoControle = [CodigoControleLista]
                                                      FROM [QuadroJw].[dbo].[ControleLista]
                                                      where TipoListaId = @TipoListaId
                                                      and Participou = 1
                                                      and Proximo = 1
                                                      Order by OrdenaFinal, CodigoControleLista DESC
                                                     END

                                                    select  @Indice = Indice from
                                                    (SELECT row_number() OVER (ORDER BY TipoListaId, OrdenaFinal, CodigoControleLista ) As Indice, CodigoControleLista 
                                                    FROM [QuadroJw].[dbo].[ControleLista]
                                                    where TipoListaId = @TipoListaId
                                                    ) As Tab
                                                    where CodigoControleLista = @CodigoControle

                                                    select @IndiceMax = IndiceMax, @IndiceMin = IndiceMin FROM (select  Max(Indice) As IndiceMax, Min(Indice) As IndiceMin from
                                                    (SELECT row_number() OVER (ORDER BY TipoListaId, OrdenaFinal, CodigoControleLista ) As Indice, CodigoControleLista 
                                                    FROM [QuadroJw].[dbo].[ControleLista]
                                                    where TipoListaId = @TipoListaId
                                                    ) As Tab2
                                                    ) As Tab3

                                                    IF (@Indice = @IndiceMax)
                                                     BEGIN
	                                                  UPDATE [dbo].[ControleLista] SET Proximo = 1 where CodigoControleLista = @CodigoControle and TipoListaId = @TipoListaId

                                                      select  @CodigoControle = CodigoControleLista from   
                                                      (SELECT row_number() OVER (ORDER BY TipoListaId, OrdenaFinal, CodigoControleLista ) As Indice, CodigoControleLista 
                                                      FROM [QuadroJw].[dbo].[ControleLista]
                                                      where TipoListaId = @TipoListaId
                                                      ) As Tab
                                                      where Indice = @IndiceMin

	                                                  UPDATE [dbo].[ControleLista] SET Participou = 0 where CodigoControleLista = @CodigoControle and TipoListaId = @TipoListaId
                                                     END
                                                    ELSE 
                                                     BEGIN
	                                                  UPDATE [dbo].[ControleLista] SET Proximo = 1 where CodigoControleLista = @CodigoControle and TipoListaId = @TipoListaId

                                                      select  @CodigoControle = CodigoControleLista from
                                                      (SELECT row_number() OVER (ORDER BY TipoListaId, OrdenaFinal, CodigoControleLista ) As Indice, CodigoControleLista 
                                                      FROM [QuadroJw].[dbo].[ControleLista]
                                                      where TipoListaId = @TipoListaId
                                                      ) As Tab
                                                      where Indice = (@Indice + 1)

	                                                  UPDATE [dbo].[ControleLista] SET Participou = 0  where CodigoControleLista = @CodigoControle and TipoListaId = @TipoListaId

                                                     END
";

        private const string LiberaMaisAntigo = @"DECLARE @IrmaoId int
                                                  SELECT TOP 1 @IrmaoId = IrmaoId FROM 
                                                  (Select QD.IrmaoId, Max(QD.Data) As Last, Sum(Participacoes) As Participacoes from QuadroDetalhe QD
                                                  INNER JOIN Quadro Q ON Q.Codigo = QD.QuadroId
                                                  INNER JOIN ControleLista CL ON CL.IrmaoId = QD.IrmaoId 
                                                  WHERE Q.TipoListaId = @TipoListaId

                                                  and Q.CongregacaoId = @CongregacaoId

                                                  and QD.IrmaoId is not null
                                                  and CL.Participou = 1
                                                  and CL.TipoListaId = @TipoListaId
                                                  GROUP BY QD.IrmaoId) As Tab 
                                                  order by Last, Participacoes;

                                                    UPDATE [dbo].[ControleLista] SET Participou = 0  where IrmaoId =  @IrmaoId and TipoListaId = @TipoListaId;
                                                    select @IrmaoId
";

        #endregion
        public void AtualizaPartipacaoIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId, bool liberaproximo)
        {
            string sql = (!liberaproximo) ? " UPDATE [dbo].[ControleLista] SET [Participou] = 1, Participacoes = Participacoes + 1  WHERE IrmaoId = @IrmaoId and TipoListaId = @TipoListaId; " :
                                                             " UPDATE [dbo].[ControleLista] SET [Participou] = 1, Participacoes = Participacoes + 1, Proximo = 1 WHERE IrmaoId = @IrmaoId and TipoListaId = @TipoListaId; ";

            //sql = sql + @" IF NOT EXISTS(SELECT 1 FROM [dbo].[ControleLista] WHERE Participou = 0 and TipoListaId = @TipoListaId)
             //                                BEGIN
              //                                UPDATE [dbo].[ControleLista] SET Participou = 0, Proximo = 0 where TipoListaId = @TipoListaId
               //                              END ";
            unitOfWork.Connection.Execute(sql,
               param: new
               {
                   @IrmaoId = irmaoId,
                   @TipoListaId = tipoListaId
               },
               transaction: unitOfWork.Transaction);
        }

        public void BackupListaAtual(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime dataInicioLista, int congregacaoId)
        {
            unitOfWork.Connection.Execute(backupListaAtualSQL,
               param: new
               {
                   @TipoListaId = tipoListaId,
                   @DataInicioLista = dataInicioLista,
                   @CongregacaoId = congregacaoId
               },
               transaction: unitOfWork.Transaction);
        }

        public void IncluirIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId, bool finalLista)
        {
            unitOfWork.Connection.Execute(insertIrmaoLista,
               param: new
               {
                   @IrmaoId = irmaoId,
                   @TipoListaId = tipoListaId,
                   @OrdenaFinal = finalLista
               },
               transaction: unitOfWork.Transaction);
        }

        public ControleLista ObterProximoListaPodeRepetir(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId)
        {
            return unitOfWork.Connection.Query<ControleLista>(@"select top 1 * from dbo.ControleLista where TipoListaId = @TipoListaId and Participou = 0 and CongregacaoId = @CongregacaoId
                                                              Order by OrdenaFinal, CodigoControleLista",
                    param: new { @TipoListaId = tipoListaId, @CongregacaoId = congregacaoId }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public ControleLista ObterProximoListaSemRepetirComFolga(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datareuniaoanterior, DateTime datereuniaoAtual, 
            DateTime dataProximaReuniao, int congregacaoId)
        {
            return unitOfWork.Connection.Query<ControleLista>(SelectProximoListaComFolga,
                    param: new { @TipoListaId = tipoListaId,
                                 @DataReuniaoAnterior = datareuniaoanterior,
                                 @DataReuniaoAtual = datereuniaoAtual,
                                 @DataProximaReuniao = dataProximaReuniao,
                                 @ListaOracao = eTipoLista.OracaoFinal,
                                 @CongregacaoId = congregacaoId
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public ControleLista ObterProximoListaSemRepetirSemFolgaParaAudioSonoro(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datareuniaoanterior, DateTime datereuniaoAtual, 
            DateTime dataProximaReuniao, int congregacaoId)
        {
            return unitOfWork.Connection.Query<ControleLista>(SelectProximoListaSemFolgaAudio,
                    param: new
                    {
                        @TipoListaId = tipoListaId,
                        @DataReuniaoAnterior = datareuniaoanterior,
                        @DataReuniaoAtual = datereuniaoAtual,
                        @DataProximaReuniao = dataProximaReuniao,
                        @ListaOracao = eTipoLista.OracaoFinal,
                        @CongregacaoId = congregacaoId
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }


        public ControleLista ObterProximoListaSemRepetirSemFolga(ref IUnitOfWork unitOfWork, int tipoListaId, DateTime datereuniaoAtual, DateTime datareuniaoanterior, 
            DateTime dataProximaReuniao, int congregacaoId)
        {
            return unitOfWork.Connection.Query<ControleLista>(SelectProximoListaSemFolga,
                    param: new
                    {
                        @TipoListaId = tipoListaId,
                        @DataReuniaoAtual = datereuniaoAtual,
                        @DataReuniaoAnterior = datareuniaoanterior,
                        @DataReuniaoProxima = dataProximaReuniao,
                        @CongregacaoId = congregacaoId
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public void RecuperaBackupListaAtual(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId)
        {
            unitOfWork.Connection.Execute(recuperaBackupLista,
               param: new
               {
                   @TipoListaId = tipoListaId,
                   @CongregacaoId = congregacaoId
               },
               transaction: unitOfWork.Transaction);
        }

        public void RemoverIrmaoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int irmaoId)
        {
            unitOfWork.Connection.Execute("DELETE FROM dbo.ControleLista WHERE TipoListaId = @TipoListaId and IrmaoId = @IrmaoId;",
               param: new
               {
                   @TipoListaId = tipoListaId,
                   @IrmaoId = irmaoId,
               },
               transaction: unitOfWork.Transaction);    
        }

        public ControleLista GetListaIrmao(ref IUnitOfWork unitOfWork, int tipoListaId, int irmao)
        {
            var param = new
            {
                @TipoListaId = tipoListaId,
                irmao
            };
            return unitOfWork.Connection.Query<ControleLista>(" select top 1 * from dbo.ControleLista where TipoListaId = @TipoListaId and IrmaoId = @irmao",
          param: param
          , transaction: unitOfWork.Transaction
      ).FirstOrDefault();
        }

        public void LiberaProximoLista(ref IUnitOfWork unitOfWork, int tipoListaId, int congregacaoId)
        {
            var id = unitOfWork.Connection.ExecuteScalar(LiberaMaisAntigo,
               param: new
               {
                   @TipoListaId = tipoListaId,
                   @CongregacaoId = congregacaoId
               },
               transaction: unitOfWork.Transaction);
        }



    }
}
