using System.Collections.Generic;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IGrupoServices
    {
        IEnumerable<Grupo> ObterGruposPorCongregacao(int congregacaoId);
    }
}
