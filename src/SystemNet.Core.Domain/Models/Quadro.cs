using Flunt.Notifications;
using System;

namespace SystemNet.Core.Domain.Models
{
    public class Quadro : Notifiable
    {
        public int Codigo { get; set; }
        public int QuadroId { get; set; }
        public DateTime DataInicioLista { get; set; }
        public DateTime DataFimLista { get; set; }
    }
}
