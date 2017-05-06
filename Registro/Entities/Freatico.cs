using System;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Freatico
    {
        public string Dia { get; set; }
        public string Hora { get; set; }
        public double? Profundidad { get; set; }
    }
}