using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidator;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Core.Domain.Querys.Grupo;
using SystemNet.Practice.Common.Resources;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;
using static SystemNet.Core.Domain.Models.Irmao;

namespace SystemNet.Business.Services
{
    public class IrmaoServices : IIrmaoServices
    {
        readonly IIrmaoRepository _repository;

        public IrmaoServices(IIrmaoRepository repository)
        {
            _repository = repository;
        }

        public Irmao Adicionar(Irmao model, int userId)
        {
            if (model.Valid)
            {
                var password = model.GeraNovaSenha(8, true);

                using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
                {
                    IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                    unitOfWork.Begin();
                    try
                    {
                        if (_repository.PesquisarporEmail(ref unitOfWork, model.Email) != null)
                        {
                            model.AddNotification(nameof(model.Email), Errors.LoginExists);
                            unitOfWork.Rollback();
                            return model;
                        }

                        // Valida se não existe um outro registro com mesmo nome
                        if (!VerificarNome(ref unitOfWork, ref model)) return model;
                        
                        model.Codigo = _repository.Inserir(ref unitOfWork, model);

                        _repository.SendEmail(model, password, true);

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

        public Irmao EsquecerSenha(string login)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {

                    var model = BuscarporLogin(ref unitOfWork, login);

                    if (model == null || model.Invalid) return model;
                    else if (model.StatusId != (int)Status.Ativo && model.StatusId != (int)Status.BloqueadoporSenha)
                    {
                        model.AddNotification(nameof(Irmao.Email), Errors.BlockedUser);
                        unitOfWork.Rollback();
                        return model;
                    }

                    var novasenha = model.GeraNovaSenha(8, false);
                    model.StatusId = (int)Status.Ativo;
                    _repository.ReiniciarSenha(ref unitOfWork, model);
                    _repository.SendEmail(model, novasenha, false);
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

        public Irmao Ativar(int id, int userId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarPorId(ref unitOfWork, id);

                    if (model == null || model.Invalid) return model;

                    _repository.Ativar(ref unitOfWork, id);
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

        public IReadOnlyCollection<Notification> Atualizar(Irmao updatemodel, int userId)
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

                        //Valida se não existe um outro registro como este mesmo nome
                        if (!VerificarNome(ref unitOfWork, ref updatemodel, updatemodel.Codigo)) return updatemodel.Notifications;

                        //Valida se não existe um outro registro com este mesmo e-mail
                        if (!VerificarEmail(ref unitOfWork, ref model, updatemodel.Email, model.Codigo)) return model.Notifications;

                        updatemodel.AtualizarDesignacao = model.AtualizarDesignacao;

                        // Verifica se é necessário atualizar as designações
                        if (!model.AtualizarDesignacao)  updatemodel.VerificaDesignacoes(model);

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

        public Irmao Autenticar(string login, string password)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarporLogin(ref unitOfWork, login);

                    if (model == null || model.Invalid) return model;
                    else if (!model.ValidarSenha(password)) return AtualizarLogin(ref unitOfWork, ref model);

                    return AtualizarLogin(ref unitOfWork, ref model);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        public Irmao AlterarSenha(string login, string UserToken, string senha, string novaSenha, string confirmacaoNovaSenha)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarporLogin(ref unitOfWork, login);

                    if (model == null || model.Invalid) return model;

                    if (model.AlteraSenhaAtual(model.Codigo, model.Senha,
                        senha, novaSenha, confirmacaoNovaSenha, Convert.ToInt32(UserToken)))
                    {
                        _repository.AlterarSenha(ref unitOfWork, model.Codigo, model.Senha);
                        unitOfWork.Commit();
                    }
                    return model;
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }

        }

        public Irmao Desativar(int id, int userId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                unitOfWork.Begin();
                try
                {
                    var model = BuscarPorId(ref unitOfWork, id);

                    if (model == null || model.Invalid) return model;

                    _repository.Desativar(ref unitOfWork, id);
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

        public IEnumerable<GetGrupoIrmao> ObterGruposComIrmaos(int congregacaoId)
        {
            using (RepositorySession dalSession = new RepositorySession(Runtime.JWInstance))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var ret = _repository.ObterGruposComIrmaos(ref unitOfWork, congregacaoId);

                    foreach (var item in ret)
                    {
                        if (item.Irmaos[0] == null)
                            item.Irmaos.Clear();
                    }

                    return ret;
                }
                catch
                {
                    throw;
                }
            }
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
        public IEnumerable<GetIrmao> ObterIrmaosPorCongregacaoPorAdmin(int congregacaoId, int userId)
        {
            using (RepositorySession dalSession = new RepositorySession("JW"))
            {
                IUnitOfWork unitOfWork = dalSession.UnitOfWork;
                try
                {
                    var ret = _repository.ObterIrmaosPorCongregacao(ref unitOfWork, congregacaoId).ToList();
                    var isAdmin = ret.Find(x => x.Codigo == userId).AcessoAdmin;
                    if (!isAdmin) return ret.FindAll(x => x.Codigo == userId);

                    return ret;
                }
                catch
                {
                    throw;
                }
            }
        }

        private Irmao AtualizarLogin(ref IUnitOfWork unitOfWork, ref Irmao model)
        {
            _repository.Login(ref unitOfWork, model);
            unitOfWork.Commit();
            return model;
        }

        private bool VerificarEmail(ref IUnitOfWork unitOfWork, ref Irmao model, string email, int? id = (int?)null)
        {
            if (email == string.Empty) return true;

            var ret = _repository.PesquisarporEmail(ref unitOfWork, email, id);

            if (ret != null)
            {
                model.AddNotification(nameof(model.Email), Errors.EmailExists);
                unitOfWork.Rollback();
                return false;
            }

            return true;
        }

        private Irmao BuscarPorId(ref IUnitOfWork unitOfWork, int id)
        {
            var ret = _repository.FindById(ref unitOfWork, id);

            if (ret == null)
            {
                ret = new Irmao();
                ret.AddNotification(nameof(ret.Codigo), string.Format(Errors.RegisterNotFound, id));
                unitOfWork.Rollback();
            }
            return ret;
        }

        private Irmao BuscarporLogin(ref IUnitOfWork unitOfWork, string email)
        {
            var ret = _repository.PesquisarporEmail(ref unitOfWork, email);

            if (ret == null)
            {
                ret = new Irmao();
                ret.AddNotification(nameof(ret.Email), Errors.UserNotFound);
                unitOfWork.Rollback();
            }
            return ret;
        }

        private bool VerificarNome(ref IUnitOfWork unitOfWork, ref Irmao model, int? id = (int?)null)
        {
            var ret = _repository.PesquisarporNome(ref unitOfWork, model.Nome, id);

            if (ret != null)
            {
                model.AddNotification(nameof(model.Nome), Errors.NameExists);
                unitOfWork.Rollback();
                return false;
            }

            return true;
        }

    }
}
