using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Core.Domain.Models
{
    public class Grupo
    {
        public int Codigo { get; set; }
        public string  Nome { get; set; }
        public int DirigenteId { get; set; }
        public int CongregacaoId { get; set; }
        public string Local { get; set; }

    }
}
