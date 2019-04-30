select * from Quadro

select I.Nome As Irmao, DE.Descricao As Evento, QD.* from QuadroDetalhe QD
inner join Quadro Q ON Q.Codigo = QD.QuadroId
LEFT join Irmao I ON QD.IrmaoId = I.Codigo
LEFT join DataEvento DE ON DE.Codigo = QD.EventoId
where Q.TipoListaId = 1 
order by Data

select I.Nome As Irmao, DE.Descricao As Evento, QD.* from QuadroDetalhe QD
inner join Quadro Q ON Q.Codigo = QD.QuadroId
LEFT join Irmao I ON QD.IrmaoId = I.Codigo
LEFT join DataEvento DE ON DE.Codigo = QD.EventoId
where Q.TipoListaId = 2
order by Data

select I.Nome As Irmao, DE.Descricao As Evento, QD.* from QuadroDetalhe QD
inner join Quadro Q ON Q.Codigo = QD.QuadroId
LEFT join Irmao I ON QD.IrmaoId = I.Codigo
LEFT join DataEvento DE ON DE.Codigo = QD.EventoId
where Q.TipoListaId = 3 
order by Data

select I.Nome As Irmao, DE.Descricao As Evento, QD.* from QuadroDetalhe QD
inner join Quadro Q ON Q.Codigo = QD.QuadroId
LEFT join Irmao I ON QD.IrmaoId = I.Codigo
LEFT join DataEvento DE ON DE.Codigo = QD.EventoId
where Q.TipoListaId = 4
order by Data

select I.Nome As Irmao, DE.Descricao As Evento, QD.* from QuadroDetalhe QD
inner join Quadro Q ON Q.Codigo = QD.QuadroId
LEFT join Irmao I ON QD.IrmaoId = I.Codigo
LEFT join DataEvento DE ON DE.Codigo = QD.EventoId
where Q.TipoListaId = 5
order by Data