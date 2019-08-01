using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IDataEventoRepository
    {
        void Inserir(ref IUnitOfWork unitOfWork, DataEvento model);
        void Apagar(ref IUnitOfWork unitOfWork, int id);
        DataEvento FindById(ref IUnitOfWork unitOfWork, int id);
        IEnumerable<DataEvento> ObterEventosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
        DataEvento ListByDate(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data);
        DataEvento PesquisarporDataeCongregacao(ref IUnitOfWork unitOfWork, DataEvento model, int? id = null);
    }
}
