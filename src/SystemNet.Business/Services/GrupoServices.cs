using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Business.Services
{
    public class GrupoServices : IGrupoServices
    {
        IGrupoRepository _repository;

        public GrupoServices(IGrupoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Grupo> ObterGruposPorCongregacao(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repository.ObterGruposPorCongregacao(ref unitOfWork, congregacaoId);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}

