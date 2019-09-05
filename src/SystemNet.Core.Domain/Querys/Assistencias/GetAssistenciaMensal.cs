using System;
using static SystemNet.Practice.Common.Values.DateValues;

namespace SystemNet.Core.Domain.Querys.Assistencias
{
    public class GetAssistenciaMensal
    {
        public int Ano { get; set; }
        public Month Mes { get; set; }
        public int AssistenciaTotal { get; set; }
        public int QuantidadeReunioes { get; set; }
        public DayOfWeek DiaReuniao { get; set; }
    }
}
