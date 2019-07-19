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
    public class GrupoRepository : IGrupoRepository
    {
        public void Inserir(ref IUnitOfWork unitOfWork, Grupo model)
        {
            unitOfWork.Connection.Execute(@" INSERT INTO [dbo].[Grupo] ([Nome], [DirigenteId], [CongregacaoId], [Local])
                                                    VALUES
                                                (@Nome, @DirigenteId, @CongregacaoId, @Local) ",
                               param: new
                               {
                                   @Nome = model.Nome,
                                   @DirigenteId = model.DirigenteId,
                                   @CongregacaoId = model.CongregacaoId,
                                   @Local = model.Local
                               },
                               transaction: unitOfWork.Transaction);
        }

        public void Atualizar(ref IUnitOfWork unitOfWork, Grupo model)
        {
            unitOfWork.Connection.Execute(@" UPDATE [dbo].[Grupo]
                                             SET [Nome] = @Nome, [DirigenteId] = @DirigenteId, [Local] = @Local
                                             WHERE Codigo = @GrupoId",
                                        param: new
                                        {
                                            @Nome = model.Nome,
                                            @DirigenteId = model.DirigenteId,
                                            @Local = model.Local,
                                            @GrupoId = model.Codigo
                                        },
                                        transaction: unitOfWork.Transaction);
        }

        public void Apagar(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute(@" DELETE FROM [dbo].[Grupo]
                                             WHERE Codigo = @GrupoId",
                                        param: new
                                        {
                                            @GrupoId = id
                                        },
                                        transaction: unitOfWork.Transaction);
        }

        public Grupo FindById(ref IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.Connection.Query<Grupo>("Select * from dbo.Grupo where Codigo = @id",
                    param: new
                    {
                        @id = id
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public Grupo PesquisarporNomeGrupo(ref IUnitOfWork unitOfWork, string nome, int? id = null)
        {
            return unitOfWork.Connection.Query<Grupo>(
                      (id != null) ? @"SELECT G.* FROM dbo.[Grupo] G 
                                     WHERE Nome = @Nome  and G.Codigo <> @Codigo " :
                                     @" SELECT G.* FROM 
                                        dbo.[Grupo] G 
                                        WHERE Nome = @Nome ",
                      param: new
                      {
                          @Nome = nome,
                          @Codigo = (IntValues.IsNullorDefault(id)) ? (int?)null : id
                      },
                      transaction: unitOfWork.Transaction
                  ).FirstOrDefault();
        }

        public IEnumerable<Grupo> ObterGruposPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<Grupo>("Select * from dbo.Grupo where CongregacaoId = @congregacao order by Nome",
                param: new
                {
                    @congregacao = congregacaoId
                }
               );
        }


    }
}
