using System;
using System.Collections.Generic;
using SystemNet.Core.Domain.enums;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Uow;

namespace SystemNet.Core.Domain.Contracts.Repositories
{
    public interface IQuadroDetalheRepository
    {
        void InsereDataQuadro(ref IUnitOfWork unitOfWork, QuadroDetalhe model);
        void ApagaDetalhesQuadro(ref IUnitOfWork unitOfWork, int QuadroId);
        List<Irmao> ObterIrmaosTipoLista(ref IUnitOfWork unitOfWork, eTipoLista tipolist, int QuadroId, DateTime data);
        QuadroDetalhe ObterUltimaReuniaoValida(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data);
        QuadroDetalhe ObterProximaReuniaoValida(ref IUnitOfWork unitOfWork, int congregacaoId, DateTime data);
    }
}
