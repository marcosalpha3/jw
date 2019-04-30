using System.Collections.Generic;
using SystemNet.Core.Domain.Querys;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IIrmaoServices 
    {
        IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(int congregacaoId);
    }
}
