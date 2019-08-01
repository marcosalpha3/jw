using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practice.Common.Resources;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;

namespace SystemNet.Business.Services
{
    public class DataEventoServices : IDataEventoServices
    {
        IDataEventoRepository _repository;

        public DataEventoServices(IDataEventoRepository repository)
        {
            _repository = repository;
        }

        public DataEvento Adicionar(DataEvento model)
        {
            if (model.Valid)
            {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {
                        if (_repository.PesquisarporDataeCongregacao(ref unitOfWork, model) != null)
                        {
                            model.AddNotification(nameof(model.Data), Errors.EventExists);
                            unitOfWork.Rollback();
                            return model;
                        }
                        _repository.Inserir(ref unitOfWork, model);
                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
            }
            return model;
        }

        public DataEvento Apagar(int codigo)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarPorId(ref unitOfWork, codigo);

                    if (model == null || model.Invalid) return model;

                    _repository.Apagar(ref unitOfWork, codigo);
                    unitOfWork.Commit();
                    return model;
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<DataEvento> ObterEventosPorCongregacao(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repository.ObterEventosPorCongregacao(ref unitOfWork, congregacaoId);
                }
                catch
                {
                    throw;
                }
            }
        }

        private DataEvento BuscarPorId(ref IUnitOfWork unitOfWork, int id)
        {
            var ret = _repository.FindById(ref unitOfWork, id);

            if (ret == null)
            {
                ret = new DataEvento();
                ret.AddNotification(nameof(ret.Codigo), string.Format(Errors.RegisterNotFound, id));
                unitOfWork.Rollback();
            }
            return ret;
        }

    }
}
