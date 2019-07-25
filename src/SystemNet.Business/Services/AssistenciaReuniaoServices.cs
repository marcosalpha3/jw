using FluentValidator;
using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys.Assistencias;
using SystemNet.Practice.Common.Values;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;

namespace SystemNet.Business.Services
{
    public class AssistenciaReuniaoServices : IAssistenciaReuniaoServices
    {
        IAssistenciaReuniaoRepository _repository;

        public AssistenciaReuniaoServices(IAssistenciaReuniaoRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyCollection<Notification> Apagar(int congregacaoId, DateTime data)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    _repository.Apagar(ref unitOfWork, data, congregacaoId);
                    unitOfWork.Commit();

                    return new AssistenciaReuniao().Notifications;
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public IReadOnlyCollection<Notification> Atualizar(AssistenciaReuniao updatemodel)
        {
            if (updatemodel.Valid)
            {
                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {
                        var model = _repository.FindByData(ref unitOfWork, updatemodel.CongregacaoId, updatemodel.Data);

                        if (model == null)
                            _repository.Inserir(ref unitOfWork, updatemodel);
                        else
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

        public List<GetDetalheAssistencia> ObterAssistenciasPorCongregacao(int congregacaoId, DateTime dataInicial, DateTime dataFinal)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var ret = _repository.ObterAssistenciasPorCongregacao(ref unitOfWork, congregacaoId, dataInicial, dataFinal);

                    for (int i = 0; i < ret.Count; i++)
                    {
                        ret[i].DiaSemana = DateValues.ObterDiaSemanaPortugues(ret[i].Data.DayOfWeek);
                    }

                    return ret;
                }
                catch
                {
                    throw;
                }
            }
        }
    
    }
}
