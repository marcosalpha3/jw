using System.Collections.Generic;

namespace SystemNet.Core.Domain.Querys.Grupo
{
    public class GetGrupoIrmao
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Dirigente { get; set; }
        public int DirigenteId { get; set; }
        public string Local { get; set; }
        public List<GetIrmaoGrupo> Irmaos { get; set; }
    }
}
