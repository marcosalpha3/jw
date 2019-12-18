using FluentValidator;
using FluentValidator.Validation;
using System;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class ExcecaoDesignacao : Notifiable
    {
        public ExcecaoDesignacao(int codigo, DateTime data, int irmaoId, string motivo)
        {
            Codigo = codigo;
            Data = data;
            IrmaoId = irmaoId;
            Motivo = motivo;


            AddNotifications(new ValidationContract()
                           .Requires()
                           .IsGreaterOrEqualsThan(Data.Date, DateTime.Now.Date.AddDays(-1), nameof(Data), Errors.InitialDateGrantCurrentDate)
                           .HasMinLen(Motivo, 3, nameof(Motivo), String.Format(Errors.Min, Errors.Reason, 3))
                           .HasMaxLen(Motivo, 50, nameof(Motivo), String.Format(Errors.Max, Errors.Reason, 50))
                           .IsGreaterThan(IrmaoId, 0, nameof(IrmaoId), Errors.BrotherRequired)
                           );
        }

        public ExcecaoDesignacao()   { }

        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public int IrmaoId { get; set; }
        public string Motivo { get; set; }
        public string IrmaoNome { get; set; }
    }
}
