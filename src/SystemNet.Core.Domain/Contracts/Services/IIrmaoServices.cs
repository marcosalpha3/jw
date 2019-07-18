using FluentValidator;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Core.Domain.Querys.Grupo;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IIrmaoServices 
    {
        IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(int congregacaoId);
        IReadOnlyCollection<Notification> Atualizar(Irmao updatemodel, int userId);
        Irmao Autenticar(string login, string password);
        Irmao Adicionar(Irmao model, int userId);
        Irmao Desativar(int id, int userId);
        Irmao Ativar(int id, int userId);
        Irmao EsquecerSenha(string login);
        Irmao AlterarSenha(string login, string UserToken, string senha, string novaSenha, string confirmacaoNovaSenha);
        IEnumerable<GetGrupoIrmao> ObterGruposComIrmaos(int congregacaoId);
    }
}
