using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemNet.Api.Models.Quadro
{
    public class NovoQuadroPersonalizado
    {
        public string Titulo { get; set; }
        public int CongregacaoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataExpiracao { get; set; }
    }
}
