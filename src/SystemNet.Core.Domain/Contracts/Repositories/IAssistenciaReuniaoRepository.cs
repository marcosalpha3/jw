using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys.Assistencias;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IAssistenciaReuniaoRepository
    {
        List<GetDetalheAssistencia> ObterAssistenciasPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime dataInicial, DateTime dataFinal);
        AssistenciaReuniao FindByData(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data);
        void Inserir(ref IUnitOfWork unitOfWork, AssistenciaReuniao model);
        void Atualizar(ref IUnitOfWork unitOfWork, AssistenciaReuniao model);
        void Apagar(ref IUnitOfWork unitOfWork, DateTime data, int congregacaoId);
        List<GetAssistenciaMensal> ObterAssistenciasMensal(ref IUnitOfWork unitOfWork, int congregacaoId);
    }
}
