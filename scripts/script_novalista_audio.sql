use QuadroJw
Go

INSERT INTO [dbo].[TipoLista]
           ([Codigo]
           ,[Descricao]
           ,[PodeRepetirDia]
           ,[QuantidadeDatas]
           ,[CongregacaoId])
     VALUES
           (6
           ,'Audio e Video'
           ,0
           ,15
           ,1)
GO
-- Danilo
Update Irmao set SistemaSonoro = 1 where Codigo = 15

-- Ricardo Navarro
Update Irmao set SistemaSonoro = 1 where Codigo = 32

INSERT INTO [dbo].[ControleLista]
           ([TipoListaId]
           ,[IrmaoId]
           ,[CongregacaoId]
           ,[Participou])
select 6, Codigo, CongregacaoId, 0 from
(select RAND(CHECKSUM(NEWID())) as SortOrder, * from Irmao where  SistemaSonoro = 1 ) As T
order by SortOrder



