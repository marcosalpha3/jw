using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Core.Domain.Models
{
    public class DataEvento
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public int CongregacaoId { get; set; }
        public bool Assembleia { get; set; }
        public bool VisitaSuperintendente { get; set; }
    }
}
