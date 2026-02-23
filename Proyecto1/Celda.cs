using System;
using System.IO;

namespace Proyecto1
{
    public class Celda 
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
    }

    public class Paciente 
    {
        public string? Nombre { get; set; }
        public int Edad { get; set; }
        public int M { get; set; } 
        public int PeriodosMax { get; set; }
        public ListaEnlazada<Celda> CelulasContagiadas { get; set; }

        public Paciente() 
        {
            CelulasContagiadas = new ListaEnlazada<Celda>();
        }

        public bool EstaContagiada(int f, int c) 
        {
            Nodo<Celda>? act = CelulasContagiadas.Primero;
            while (act != null) 
            {
                if (act.Valor != null && act.Valor.Fila == f && act.Valor.Columna == c) return true;
                act = act.Siguiente;
            }
            return false;
        }
    }
}