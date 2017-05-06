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
            Profundidad = 10;
            Equipo = "";
            Operador = "";
            Inclinacion = "";
            CotaInicio = "";
            CoordenadaEste = "";
            CoordenadaNorte = "";
            Escala = 75;
            Ensayos = new List<Ensayo>();
            Muestras = new List<Muestra>();
            Freaticos = new List<Freatico>();
            Observaciones = "";
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public double? Profundidad { get; set; }
        public string Equipo { get; set; }
        public string Operador { get; set; }
        public string Inclinacion { get; set; }
        public string CotaInicio { get; set; }
        public string CoordenadaNorte { get; set; }
        public string CoordenadaEste { get; set; }
        public int Escala { get; set; }
        public List<Ensayo> Ensayos { get; set; }
        public List<Muestra> Muestras { get; set; }
        public List<Freatico> Freaticos { get; set; }
        public string Observaciones { get; set; }
    }
}