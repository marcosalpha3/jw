using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IExcecaoDesignacaoServices
    {
        ExcecaoDesignacao Inserir(ExcecaoDesignacao model);
        ExcecaoDesignacao Apagar(int id);
        IEnumerable<ExcecaoDesignacao> ObterExcecaoPorCongregacao(int congregacaoId);
        void InserirExcecaoAteData(int irmaoId, DayOfWeek dayofWeekExeption, DateTime toDate);
    }
}
