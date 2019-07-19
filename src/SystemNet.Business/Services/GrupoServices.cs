using FluentValidator;
using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practice.Common.Resources;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;

namespace SystemNet.Business.Services
{
    public class GrupoServices : IGrupoServices
    {
        IGrupoRepository _repository;
        IIrmaoRepository _repositoryIrmao;

        public GrupoServices(IGrupoRepository repository,
                            IIrmaoRepository repositoryIrmao)
        {
            _repository = repository;
            _repositoryIrmao = repositoryIrmao;
        }


        public Grupo Adicionar(Grupo model)
        {
            if (model.Valid)
            {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {
                        if (_repository.PesquisarporNomeGrupo(ref unitOfWork, model.Nome) != null)
                        {
                            model.AddNotification(nameof(model.Nome), Errors.NameExists);
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

        public IReadOnlyCollection<Notification> Atualizar(Grupo updatemodel)
        {
            if (updatemodel.Valid)
            {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {

                        var model = BuscarPorId(ref unitOfWork, updatemodel.Codigo);

                        if (model == null || model.Invalid) return model.Notifications;

                        if (_repository.PesquisarporNomeGrupo(ref unitOfWork, updatemodel.Nome, model.Codigo) != null)
                        {
                            model.AddNotification(nameof(model.Nome), Errors.NameExists);
                            unitOfWork.Rollback();
                            return model.Notifications;
                        }

                        _repository.Atualizar(ref unitOfWork, updatemodel);

                        unitOfWork.Commit();

                        return updatemodel.Notifications;

                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }


            }
            return updatemodel.Notifications;
        }

        public Grupo Apagar(int grupoAtual, int grupoNovo)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarPorId(ref unitOfWork, grupoAtual);

                    if (model == null || model.Invalid) return model;

                    _repositoryIrmao.UpdateGrupoCampo(ref unitOfWork, grupoAtual, grupoNovo);
                    _repository.Apagar(ref unitOfWork, grupoAtual);
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

        private Grupo BuscarPorId(ref IUnitOfWork unitOfWork, int id)
        {
            var ret = _repository.FindById(ref unitOfWork, id);

            if (ret == null)
            {
                ret = new Grupo();
                ret.AddNotification(nameof(ret.Codigo), string.Format(Errors.RegisterNotFound, id));
                unitOfWork.Rollback();
            }
            return ret;
        }

    }
}

