using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IGrupoRepository
    {
        IEnumerable<Grupo> ObterGruposPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
        void Inserir(ref IUnitOfWork unitOfWork, Grupo model);
        void Atualizar(ref IUnitOfWork unitOfWork, Grupo model);
        Grupo FindById(ref IUnitOfWork unitOfWork, int id);
        Grupo PesquisarporNomeGrupo(ref IUnitOfWork unitOfWork, string nome, int? id = null);
        void Apagar(ref IUnitOfWork unitOfWork, int id);
    }
}
