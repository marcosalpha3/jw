using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Business.Services
{
    public class IrmaoServices : IIrmaoServices
    {
        IIrmaoRepository _repository;

        public IrmaoServices(IIrmaoRepository repository)
        {
            _repository = repository;
        }
        
        public IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repository.ObterIrmaosPorCongregacao(ref unitOfWork, congregacaoId);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
