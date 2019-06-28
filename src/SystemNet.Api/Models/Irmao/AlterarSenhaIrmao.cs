using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemNet.Api.Models.Irmao
{
    public class AlterarSenhaIrmao
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmacaoNovaSenha { get; set; }
    }
}
