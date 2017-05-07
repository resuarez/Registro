using System;
using System.Collections.Generic;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Sondeo
    {
        public static int Cont;

        public Sondeo()
        {
            Id = ++Cont;
            Nombre = $"Sondeo N°{Id}";
            FechaInicio = DateTime.Today;
            FechaFinal = DateTime.Today;
            ProfundidadFinal = 10;
            Equipo = "";
            Operador = "";
            InclinacionVertical = "";
            CoordenadaEste = "";
            CoordenadaNorte = "";
            Escala = 20;
            Ensayos = new List<Ensayo>();
            Muestras = new List<Muestra>();
            Freaticos = new List<Freatico>();
            Observaciones = "B = BOLSA    SH = SHELBY   SS = SPT    NQ = BARRENA";
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public double? ProfundidadFinal { get; set; }
        public string Equipo { get; set; }
        public string Localizacion { get; set; }
        public string Operador { get; set; }
        public string InclinacionVertical { get; set; }
        public string CoordenadaNorte { get; set; }
        public string CoordenadaEste { get; set; }
        public int Escala { get; set; }
        public List<Ensayo> Ensayos { get; set; }
        public List<Muestra> Muestras { get; set; }
        public List<Freatico> Freaticos { get; set; }
        public string Observaciones { get; set; }
    }
}