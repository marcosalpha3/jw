using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Core.Domain.Querys.Grupo;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IIrmaoRepository
    {
        Irmao FindById(ref IUnitOfWork unitOfWork, int id);
        void AtualizaAtivarProximaLista(ref IUnitOfWork unitOfWork, int id);
        void AtualizaDesativarProximaLista(ref IUnitOfWork unitOfWork, int id);
        IEnumerable<Irmao> ObterIrmaosADesativarOuAtivar(ref IUnitOfWork unitOfWork, int congregacaoId);
        IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
        int Inserir(ref IUnitOfWork unitOfWork, Irmao model);
        void Atualizar(ref IUnitOfWork unitOfWork, Irmao model);
        void Desativar(ref IUnitOfWork unitOfWork, int codigo);
        void Ativar(ref IUnitOfWork unitOfWork, int codigo);
        Irmao PesquisarporEmail(ref IUnitOfWork unitOfWork, string email, int? id = (int?)null);
        Irmao PesquisarporNome(ref IUnitOfWork unitOfWork, string nome, int? id = (int?)null);
        void SendEmail(Irmao model, string password, bool newUser);
        void Login(ref IUnitOfWork unitOfWork, Irmao model);
        void ReiniciarSenha(ref IUnitOfWork unitOfWork, Irmao model);
        void AlterarSenha(ref IUnitOfWork unitOfWork, int Id, string senha);
        IEnumerable<GetGrupoIrmao> ObterGruposComIrmaos(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
