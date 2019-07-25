using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Practice.Common.Values
{
    public static class DateValues
    {
        public static string ObterDiaSemanaPortugues(DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Friday:
                    return "Sexta Feira";
                case DayOfWeek.Monday:
                    return "Segunda Feira";
                case DayOfWeek.Saturday:
                    return "Sabado";
                case DayOfWeek.Sunday:
                    return "Domingo";
                case DayOfWeek.Thursday:
                    return "Quinta Feira";
                case DayOfWeek.Tuesday:
                    return "Terça Feira";
                case DayOfWeek.Wednesday:
                    return "Quarta Feira";
                default:
                    return "";
            }
        }
    }
}
