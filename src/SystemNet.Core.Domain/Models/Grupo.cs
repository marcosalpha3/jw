using FluentValidator;
using FluentValidator.Validation;
using System;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class Grupo : Notifiable
    {
        public Grupo(int codigo, string nome, int dirigenteId, int congregacaoId, string local)
        {
            Codigo = codigo;
            Nome = nome;
            DirigenteId = dirigenteId;
            CongregacaoId = congregacaoId;
            Local = local;

            AddNotifications(new ValidationContract()
                           .Requires()
                           .HasMinLen(Nome, 3, nameof(Nome), String.Format(Errors.MinName, 3))
                           .HasMaxLen(Nome, 50, nameof(Nome), String.Format(Errors.MaxName, 50))
                           .IsGreaterThan(DirigenteId, 0, nameof(DirigenteId), Errors.LeaderRequired)
                           .IsGreaterThan(CongregacaoId, 0, nameof(CongregacaoId), Errors.CongreagationRequired)
                           );
        }

        public Grupo() { }

        public int Codigo { get; set; }
        public string  Nome { get; set; }
        public int DirigenteId { get; set; }
        public int CongregacaoId { get; set; }
        public string Local { get; set; }

    }
}
