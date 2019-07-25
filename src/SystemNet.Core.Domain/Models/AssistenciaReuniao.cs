using FluentValidator;
using FluentValidator.Validation;
using System;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class AssistenciaReuniao : Notifiable
    {

        public AssistenciaReuniao(DateTime data, int congregacaoId, short assistenciaParte1, short assistenciaParte2)
        {
            Data = data;
            CongregacaoId = congregacaoId;
            AssistenciaParte1 = assistenciaParte1;
            AssistenciaParte2 = assistenciaParte2;

            AddNotifications(new ValidationContract()
                           .Requires()
                           .IsGreaterThan(CongregacaoId, 0, nameof(CongregacaoId), Errors.CongreagationRequired)

                           .IsGreaterThan(AssistenciaParte1, 0, nameof(AssistenciaParte1), Errors.AssistanceRequired)
                           .IsLowerOrEqualsThan(AssistenciaParte1, 300, nameof(AssistenciaParte1), String.Format(Errors.MaxAssistance, 300))
                           .IsLowerOrEqualsThan(AssistenciaParte2, 301, nameof(AssistenciaParte2), String.Format(Errors.MaxAssistance, 300))
                           .IsLowerThan(Data, DateTime.Now.AddDays(1), nameof(Data), Errors.InvalidDate)
                           );

        }

        public AssistenciaReuniao() { }

        public DateTime Data { get; set; }
        public int CongregacaoId { get; set; }
        public Int16 AssistenciaParte1 { get; set; }
        public Int16 AssistenciaParte2 { get; set; }

    }
}
