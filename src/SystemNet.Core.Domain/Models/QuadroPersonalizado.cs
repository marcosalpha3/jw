using FluentValidator;
using FluentValidator.Validation;
using System;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class QuadroPersonalizado : Notifiable
    {
        public QuadroPersonalizado()    {}
        public QuadroPersonalizado(string url, string titulo, int congregacaoId, DateTime dataExpiracao, DateTime dataInicio, bool ativoStorage)
        {
            Url = url;
            Titulo = titulo;
            CongregacaoId = congregacaoId;
            DataExpiracao = dataExpiracao;
            DataInicio = dataInicio;
            AtivoStorage = ativoStorage;

            AddNotifications(new ValidationContract()
                           .Requires()
                           .HasMinLen(Titulo, 3, nameof(Titulo), String.Format(Errors.Min, Errors.Titulo, 3))
                           .HasMaxLen(Titulo, 50, nameof(Titulo), String.Format(Errors.Titulo, Errors.Max, 50))
                           .IsGreaterThan(CongregacaoId, 0, nameof(CongregacaoId), Errors.CongreagationRequired)
                           .IsGreaterOrEqualsThan(DataInicio.Date, DateTime.Now.Date, nameof(DataInicio), Errors.InitialDateGrantCurrentDate)
                           .IsGreaterOrEqualsThan(DataExpiracao, DataInicio, nameof(DataExpiracao), Errors.ExpirationDateGrantInitialDate)
                           );

        }

        public string Url { get; set; }
        public string Titulo { get; set; }
        public int CongregacaoId { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime DataInicio { get; set; }
        public bool AtivoStorage { get; set; }

    }   
}
