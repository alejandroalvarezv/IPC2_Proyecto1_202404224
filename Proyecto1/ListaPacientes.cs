namespace Proyecto1
{
    public class NodoPaciente {
        public Paciente? Valor { get; set; }
        public NodoPaciente? Siguiente { get; set; }
        public NodoPaciente(Paciente valor) { this.Valor = valor; }
    }

    public class ListaPaciente {
        public NodoPaciente? Primero { get; set; }
        public void Insertar(Paciente valor) {
            NodoPaciente nuevo = new NodoPaciente(valor);
            if (Primero == null) Primero = nuevo;
            else {
                NodoPaciente aux = Primero;
                while (aux.Siguiente != null) aux = aux.Siguiente;
                aux.Siguiente = nuevo;
            }
        }
        public void Limpiar() { Primero = null; }
    }
}