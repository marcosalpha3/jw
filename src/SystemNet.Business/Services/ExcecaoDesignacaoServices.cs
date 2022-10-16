using System;
using System.Collections.Generic;
using System.Diagnostics;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practice.Common.Resources;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;

namespace SystemNet.Business.Services
{
    public class ExcecaoDesignacaoServices : IExcecaoDesignacaoServices
    {
        readonly IExcecaoDesignacaoRepository _repository;

        public ExcecaoDesignacaoServices(IExcecaoDesignacaoRepository repository)
        {
            _repository = repository;
        }

        public ExcecaoDesignacao Apagar(int id)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarPorId(ref unitOfWork, id);

                    if (model == null || model.Invalid) return model;

                    _repository.Apagar(ref unitOfWork, id);
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

        public ExcecaoDesignacao Inserir(ExcecaoDesignacao model)
        {
            if (model.Valid)
            {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {
                        if (_repository.ExisteExcecaoDesignacaoPorIrmaoEData(ref unitOfWork, model.IrmaoId, model.Data))
                        {
                            model.AddNotification(nameof(model.Data), Errors.ExceptionsExists);
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

        public void InserirExcecaoAteData(int irmaoId, DayOfWeek dayofWeekExeption, DateTime toDate)
        {
            var currentDate = DateTime.Now.Date;
            while (currentDate <= toDate)
            {
                if (currentDate.DayOfWeek == dayofWeekExeption)
                    try
                    {
                        Inserir(new ExcecaoDesignacao(int.MinValue, currentDate, irmaoId, "Acompanha reunião zoom"));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Já registrado {ex.Message}");
                    }
                currentDate = currentDate.AddDays(1);
            }
        }

        public IEnumerable<ExcecaoDesignacao> ObterExcecaoPorCongregacao(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    return _repository.ObterExcecaoPorCongregacao(ref unitOfWork, congregacaoId);
                }
                catch
                {
                    throw;
                }
            }
        }

        private ExcecaoDesignacao BuscarPorId(ref IUnitOfWork unitOfWork, int id)
        {
            var ret = _repository.FindById(ref unitOfWork, id);

            if (ret == null)
            {
                ret = new ExcecaoDesignacao();
                ret.AddNotification(nameof(ret.Codigo), string.Format(Errors.RegisterNotFound, id));
                unitOfWork.Rollback();
            }
            return ret;
        }
    }
}
