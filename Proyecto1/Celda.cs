using System;

namespace Proyecto1
{
    public class Celda 
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
    }

    public class Paciente 
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int M { get; set; } // Tamaño de la rejilla (múltiplo de 10) [cite: 28]
        public int PeriodosMax { get; set; } // Límite de períodos a evaluar [cite: 97]
        public ListaEnlazada<Celda> CelulasContagiadas { get; set; }

        public Paciente() 
        {
            CelulasContagiadas = new ListaEnlazada<Celda>();
        }
    }
}