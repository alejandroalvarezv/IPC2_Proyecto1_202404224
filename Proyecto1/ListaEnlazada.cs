using System;
namespace Proyecto1
{
    public class ListaEnlazada<T> 
    {
        public Nodo<T>? Primero { get; set; }
        private int _conteo = 0; 

        public int Conteo => _conteo;

        public void Insertar(T valor) 
        {
            Nodo<T> nuevo = new Nodo<T>(valor);
            if (Primero == null) 
            {
                Primero = nuevo;
            } 
            else 
            {
                Nodo<T> aux = Primero;
                while (aux.Siguiente != null) 
                {
                    aux = aux.Siguiente;
                }
                aux.Siguiente = nuevo;
            }
            _conteo++;
        }

        public void Limpiar() 
        {
            Primero = null;
            _conteo = 0;
        }
    }
}