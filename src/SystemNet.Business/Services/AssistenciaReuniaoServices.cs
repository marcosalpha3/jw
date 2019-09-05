using FluentValidator;
using System;
using System.Collections.Generic;
using System.Linq;
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
        ICongregacaoRepository _repositoryCongregacao;

        public AssistenciaReuniaoServices(IAssistenciaReuniaoRepository repository,
                                          ICongregacaoRepository repositoryCongregacao)
        {
            _repository = repository;
            _repositoryCongregacao = repositoryCongregacao;
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

        public GetGraficoAssistenciaMensal ObterAssistenciasMensal(int congregacaoId)
        {
            string[] background = { "rgba(75, 112, 0, 0.6)", "rgba(0, 3, 205, 0.6)"} ;
            string[] borderColor = { "rgba(75, 112, 0, 1)", "rgba(0, 3, 205, 1)" };
            const int borderWidth = 1;

            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var congregacao = _repositoryCongregacao.ListAll(ref unitOfWork).FirstOrDefault(x => x.Codigo == congregacaoId);
                    var ret = _repository.ObterAssistenciasMensal(ref unitOfWork, congregacaoId);
                    var model = new GetGraficoAssistenciaMensal
                    {
                        Labels = new List<string>(),
                        Datasets = new List<Dataset>()
                    };
                    var label = "";

                    for (int i = 0; i < ret.Count; i++)
                    {
                        label = DateValues.ObterMesPortugues(ret[i].Mes, ret[i].Ano);
                        if (!model.Labels.Contains(label))
                            model.Labels.Add(label);

                        if (!model.Datasets.Any(x => x.Label == DateValues.ObterDiaSemanaPortugues(ret[i].DiaReuniao)))
                        {
                            model.Datasets.Add(new Dataset
                            {
                                BackgroundColor = (ret[i].DiaReuniao ==  congregacao.DiaReuniaoServico) ? background[0] : background[1],
                                BorderColor = (ret[i].DiaReuniao == congregacao.DiaReuniaoServico) ? borderColor[0] : borderColor[1],
                                BorderWidth = borderWidth,
                                Label = DateValues.ObterDiaSemanaPortugues(ret[i].DiaReuniao),
                                Data = ret.Where(x => x.DiaReuniao == ret[i].DiaReuniao).Select(c => c.AssistenciaTotal / c.QuantidadeReunioes).ToList()
                            });
                        }
                    }

                    return model;
                }
                catch
                {
                    throw;
                }
            }
        }



    }
}
