using System;

namespace SystemNet.Core.Domain.Querys.Assistencias
{
    public class GetDetalheAssistencia
    {
        public DateTime Data { get; set; }
        public Int16 AssistenciaParte1 { get; set; }
        public Int16 AssistenciaParte2 { get; set; }
        public string DiaSemana { get; set; }
    }
}
