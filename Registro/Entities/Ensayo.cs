using System;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Ensayo
    {
        public double? ProfundidadArriba { get; set; }
        public double? ProfundidadAbajo { get; set; }
        public double? LongitudPerforada { get; set; }
        public double? LongitudRecobrada { get; set; }
        public double? Recuperacion { get; set; }
        public string Tipo { get; set; }
        public int? N1 { get; set; }
        public int? P1 { get; set; }
        public bool R1 { get; set; }
        public int? N2 { get; set; }
        public int? P2 { get; set; }
        public bool R2 { get; set; }
        public int? N3 { get; set; }
        public int? P3 { get; set; }
        public bool R3 { get; set; }
        public double? RQD { get; set; }
    }
}