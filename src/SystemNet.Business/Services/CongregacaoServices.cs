using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Business.Services
{
    public class CongregacaoServices : ICongregacaoServices
    {
        ICongregacaoRepository _repository;

        public CongregacaoServices(ICongregacaoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Congregacao> ObterTodos()
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repository.ListAll(ref unitOfWork);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
