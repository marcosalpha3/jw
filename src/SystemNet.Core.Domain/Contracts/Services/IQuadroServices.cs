using FluentValidator;
using System.Collections.Generic;
using SystemNet.Core.Domain.Querys;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IQuadroServices
    {
        IReadOnlyCollection<Notification> GeraLista();
        IReadOnlyCollection<Notification> RegerarListaAtual();
        List<GetQuadroDesignacaoMecanica> ObterListaAtualDesignacoesMecanicas(int congregacaoId);
        List<GetQuadroDesignacaoMecanica> ObterProximaListaDesignacoesMecanicas(int congregacaoId);
    }
}
