using System;

namespace Proyecto1
{
    public class Nodo<T> 
    {
        public T Valor { get; set; }
        public Nodo<T> Siguiente { get; set; }

        public Nodo(T valor) 
        {
            this.Valor = valor;
            this.Siguiente = null;
        }
    }
}