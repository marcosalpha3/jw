using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IExcecaoDesignacaoRepository
    {
        void Inserir(ref IUnitOfWork unitOfWork, ExcecaoDesignacao model);
        void Apagar(ref IUnitOfWork unitOfWork, int id);
        ExcecaoDesignacao FindById(ref IUnitOfWork unitOfWork, int id);
        IEnumerable<ExcecaoDesignacao> ObterExcecaoPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId);
        bool ExisteExcecaoDesignacaoPorIrmaoEData(ref IUnitOfWork unitOfWork, int irmaoId, DateTime data);
    }
}
