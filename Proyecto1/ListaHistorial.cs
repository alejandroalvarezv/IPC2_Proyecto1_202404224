namespace Proyecto1
{
    public class NodoHistorial 
    {
        public string Huella { get; set; }
        public int NumeroPeriodo { get; set; }
        public NodoHistorial? Siguiente { get; set; }

        public NodoHistorial(string huella, int periodo) 
        {
            this.Huella = huella;
            this.NumeroPeriodo = periodo;
            this.Siguiente = null;
        }
    }

    public class ListaHistorial 
    {
        public NodoHistorial? Primero { get; set; }

        public void Insertar(string huella, int periodo) 
        {
            NodoHistorial nuevo = new NodoHistorial(huella, periodo);
            if (Primero == null) Primero = nuevo;
            else 
            {
                NodoHistorial aux = Primero;
                while (aux.Siguiente != null) aux = aux.Siguiente;
                aux.Siguiente = nuevo;
            }
        }

        public int BuscarRepeticion(string huellaActual) 
        {
            NodoHistorial? aux = Primero;
            while (aux != null) 
            {
                if (aux.Huella == huellaActual) return aux.NumeroPeriodo;
                aux = aux.Siguiente;
            }
            return -1; 
        }

        public void Limpiar() { Primero = null; }
    }
}