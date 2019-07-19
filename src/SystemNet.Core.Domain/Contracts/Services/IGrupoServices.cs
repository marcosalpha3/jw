using FluentValidator;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IGrupoServices
    {
        IEnumerable<Grupo> ObterGruposPorCongregacao(int congregacaoId);
        Grupo Adicionar(Grupo model);
        IReadOnlyCollection<Notification> Atualizar(Grupo updatemodel);
        Grupo Apagar(int grupoAtual, int grupoNovo);
    }
}
