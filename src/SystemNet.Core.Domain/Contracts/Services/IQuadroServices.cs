using FluentValidator;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Practices.Data.Storage.Models;

namespace SystemNet.Core.Domain.Contracts.Services
{
    public interface IQuadroServices
    {
        IReadOnlyCollection<Notification> GeraLista();
        IReadOnlyCollection<Notification> RegerarListaAtual(int congregacaoAtual);
        List<GetQuadroDesignacaoMecanica> ObterListaDesignacoesMecanicas(int congregacaoId);
        Task<QuadroPersonalizado> NovoQuadroPersonalizado(QuadroPersonalizado model, StorageConfig config, IFormFile file);
        Task<List<QuadroPersonalizado>> ApagarStorageNaoUtilizado(StorageConfig config);
        List<QuadroPersonalizado> ObterQuadrosPersonalizados(int congregacaoId);
        Task<QuadroPersonalizado> ApagarQuadroPersonalizado(int congregacaoId, string url, StorageConfig config);
    }
}
