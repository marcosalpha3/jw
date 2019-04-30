using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IDataEventoRepository
    {
       DataEvento ListByDate(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data);
    }
}
