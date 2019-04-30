USE [QuadroJw]
GO


SELECT RAND(CHECKSUM(NEWID())) as SortOrder, * 
FROM  Irmao
ORDER BY SortOrder

USE [QuadroJw]
GO

INSERT INTO [dbo].[ControleLista]
           ([TipoListaId]
           ,[IrmaoId]
           ,[CongregacaoId]
           ,[Participou])
select 2, Codigo, CongregacaoId, 0 from
(select RAND(CHECKSUM(NEWID())) as SortOrder, * from Irmao where  Microfonista = 1 ) As T
order by SortOrder

DELETE FROM ControleLista WHERE TipoListaId = 2


SELECT TL.Descricao, I.Nome FROM ControleLista CL
INNER JOIN TipoLista TL ON TL.Codigo = CL.TipoListaId
INNER JOIN Irmao I ON I.Codigo = CL.IrmaoId
ORDER BY CL.CodigoControleLista

