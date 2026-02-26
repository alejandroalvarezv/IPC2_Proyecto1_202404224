namespace Proyecto1
{
    public class NodoCelda {
        public Celda? Valor { get; set; }
        public NodoCelda? Siguiente { get; set; }
        public NodoCelda(Celda valor) { this.Valor = valor; }
    }

    public class ListaCelda {
        public NodoCelda? Primero { get; set; }
        public void Insertar(Celda valor) {
            NodoCelda nuevo = new NodoCelda(valor);
            if (Primero == null) Primero = nuevo;
            else {
                NodoCelda aux = Primero;
                while (aux.Siguiente != null) aux = aux.Siguiente;
                aux.Siguiente = nuevo;
            }
        }
    }
}