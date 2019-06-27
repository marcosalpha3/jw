using System.Collections.Generic;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface ICongregacaoServices
    {
        IEnumerable<Congregacao> ObterTodos();
    }
}
