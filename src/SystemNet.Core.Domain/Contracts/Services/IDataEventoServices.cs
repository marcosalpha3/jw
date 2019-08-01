using FluentValidator;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IDataEventoServices
    {
        IEnumerable<DataEvento> ObterEventosPorCongregacao(int congregacaoId);
        DataEvento Adicionar(DataEvento model);
        DataEvento Apagar(int codigo);
    }
}
