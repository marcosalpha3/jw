using FluentValidator;
using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys.Assistencias;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IAssistenciaReuniaoServices
    {
        List<GetDetalheAssistencia> ObterAssistenciasPorCongregacao(int congregacaoId, DateTime dataInicial, DateTime dataFinal);
        IReadOnlyCollection<Notification> Atualizar(AssistenciaReuniao updatemodel);
        IReadOnlyCollection<Notification> Apagar(int congregacaoId, DateTime data);
    }
}
