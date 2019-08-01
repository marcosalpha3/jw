using FluentValidator;
using FluentValidator.Validation;
using System;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class DataEvento : Notifiable
    {
        public DataEvento(int codigo, DateTime data, string descricao, int congregacaoId, string tipoEvento)
        {
            Codigo = codigo;
            Data = data;
            Descricao = descricao;
            CongregacaoId = congregacaoId;

            if (tipoEvento == "Assembleia")
            {
                Assembleia = true;
                VisitaSuperintendente = false;
            }
            else
            {
                Assembleia = false;
                VisitaSuperintendente = true;
            }

            AddNotifications(new ValidationContract()
                           .Requires()
                           .IsGreaterOrEqualsThan(Data.Date, DateTime.Now.Date.AddDays(-1), nameof(Data), Errors.InitialDateGrantCurrentDate)
                           .HasMinLen(Descricao, 3, nameof(Descricao), String.Format(Errors.Min, Errors.EventDescription, 3))
                           .HasMaxLen(Descricao, 100, nameof(Descricao), String.Format(Errors.Max, Errors.EventDescription, 100))
                           .IsGreaterThan(CongregacaoId, 0, nameof(CongregacaoId), Errors.CongreagationRequired)
                           );
        }

        public DataEvento()  { }

        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public int CongregacaoId { get; set; }
        public bool Assembleia { get; set; }
        public bool VisitaSuperintendente { get; set; }
    }
}
