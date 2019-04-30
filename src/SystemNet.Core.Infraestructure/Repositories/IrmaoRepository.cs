using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class IrmaoRepository : IIrmaoRepository
    {
        public void AtualizaAtivarProximaLista(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute("UPDATE [dbo].[Irmao]  SET [Ativo] = 1, [AtivarProximaLista] = 0  WHERE Codigo = @id",
               param: new
               {
                   @id = id
               },
               transaction: unitOfWork.Transaction);
        }

        public void AtualizaDesativarProximaLista(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute(@"UPDATE [dbo].[Irmao]  SET [Ativo] = 0, [DesativarProximaLista] = 0, Indicador = 0, Microfonista = 0, LeitorSentinela = 0,  
            LeitorEstudoLivro = 0, SistemaSonoro = 0, OracaoFinal = 0, PresidenteConferencia = 0, Carrinho = 0 WHERE Codigo = @id",
            param: new
            {
                @id = id
            },
            transaction: unitOfWork.Transaction);
        }

        public Irmao FindById(ref IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.Connection.Query<Irmao>("Select * from dbo.Irmao where Codigo = @id",
                    param: new
                    {
                        @id = id
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public IEnumerable<Irmao> ObterIrmaosADesativarOuAtivar(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<Irmao>("Select * from dbo.Irmao where [AtivarProximaLista] = 1 or [DesativarProximaLista] = 1 or [AtualizarDesignacao] = 1 and CongregacaoId = @congregacao",
                param: new 
                {
                    @congregacao = congregacaoId
                }
                   , transaction: unitOfWork.Transaction
               );
        }

        public IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<GetIrmao>("Select * from dbo.Irmao where CongregacaoId = @congregacao order by nome",
                param: new
                {
                    @congregacao = congregacaoId
                }
               );
        }
    }
}
