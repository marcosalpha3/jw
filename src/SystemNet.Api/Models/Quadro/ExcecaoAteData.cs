using System;

namespace SystemNet.Api.Models.Quadro
{
    public class ExcecaoAteData
    {
        public DateTime AteData { get; set; }
        public int IrmaoId { get; set; }
        public DayOfWeek Dia { get; set; }
        public string Motivo { get; set; }
    }
}
