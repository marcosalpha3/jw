using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Practice.Common.Values
{
    public static class DateValues
    {
        public enum Month : byte { Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, Jun = 6, Jul = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12 };


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

        public static string ObterMesPortugues(Month mes, int ano)
        {
            switch (mes)
            {
                case Month.Jan:
                    return $"Jan/{ano}";
                case Month.Feb:
                    return $"Fev/{ano}";
                case Month.Mar:
                    return $"Mar/{ano}";
                case Month.Apr:
                    return $"Abr/{ano}";
                case Month.May:
                    return $"Mai/{ano}";
                case Month.Jun:
                    return $"Jun/{ano}";
                case Month.Jul:
                    return $"Jul/{ano}";
                case Month.Aug:
                    return $"Ago/{ano}";
                case Month.Sep:
                    return $"Set/{ano}";
                case Month.Oct:
                    return $"Out/{ano}";
                case Month.Nov:
                    return $"Nov/{ano}";
                case Month.Dec:
                    return $"Dez/{ano}";
                default:
                    return "";
            }
        }


    }
}
