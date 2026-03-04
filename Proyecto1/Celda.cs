using System;
using System.IO;
using System.Diagnostics;
using System.Text;

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
        public ListaCelda CelulasContagiadas { get; set; }

        public string Diagnostico { get; set; } = "Leve";
        public int n { get; set; } = 0;
        public int n1 { get; set; } = 0;

        public Paciente() 
        {
            CelulasContagiadas = new ListaCelda();
        }

        public bool EstaContagiada(int f, int c) {
            NodoCelda? act = CelulasContagiadas.Primero;
            while (act != null) {
                if (act.Valor != null && act.Valor.Fila == f && act.Valor.Columna == c) return true;
                act = act.Siguiente;
            }
            return false;
        }

        private int ContarVecinos(int f, int c) {
            int cont = 0;
            for (int i = f - 1; i <= f + 1; i++) {
                for (int j = c - 1; j <= c + 1; j++) {
                    if (i == f && j == c) continue;
                    if (EstaContagiada(i, j)) cont++;
                }
            }
            return cont;
        }

        public int ContarCelulasContagiadas() {
            int contador = 0;
            NodoCelda? actual = CelulasContagiadas.Primero;
            while (actual != null) {
                contador++;
                actual = actual.Siguiente;
            }
            return contador;
        }

        public void MostrarEstadisticas(int periodo) {
            int contagiadas = ContarCelulasContagiadas();
            int totales = M * M;
            int sanas = totales - contagiadas;

            Console.WriteLine($"--- Periodo: {periodo} ---");
            Console.WriteLine($"Células Contagiadas: {contagiadas}");
            Console.WriteLine($"Células Sanas: {sanas}");
            Console.WriteLine("-------------------------");
        }

        public string GenerarHuellaDigital() {
            StringBuilder sb = new StringBuilder();
            for (int f = 1; f <= M; f++) {
                for (int c = 1; c <= M; c++) {
                    sb.Append(EstaContagiada(f, c) ? "1" : "0");
                }
            }
            return sb.ToString();
        }

        public void Evolucionar() {
            ListaCelda proxima = new ListaCelda();
            for (int f = 1; f <= this.M; f++) {
                for (int c = 1; c <= this.M; c++) {
                    int v = ContarVecinos(f, c);
                    bool viva = EstaContagiada(f, c);
                    if ((viva && (v == 2 || v == 3)) || (!viva && v == 3)) {
                        proxima.Insertar(new Celda { Fila = f, Columna = c });
                    }
                }
            }
            this.CelulasContagiadas = proxima;
        }

        // MÉTODO ACTUALIZADO PARA GENERAR REJILLA COMO EL PDF
        public void GenerarGrafica(int periodo) {
            string nombreBase = $"Paciente_{Nombre}_T{periodo}";
            try {
                using (StreamWriter sw = new StreamWriter(nombreBase + ".dot")) {
                    sw.WriteLine("digraph G {");
                    // Definimos el nodo como una tabla (rejilla)
                    sw.WriteLine("  node [shape=plaintext];");
                    sw.WriteLine($"  label=\"Paciente: {Nombre} - Periodo: {periodo}\";");
                    sw.WriteLine("  labelloc=\"t\";"); // Etiqueta en la parte superior

                    sw.WriteLine("  tabla [label=<");
                    sw.WriteLine("    <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"10\">");

                    for (int f = 1; f <= M; f++) {
                        sw.WriteLine("      <TR>");
                        for (int c = 1; c <= M; c++) {
                            // Si la celda está contagiada, fondo rojo; si no, fondo blanco
                            string color = EstaContagiada(f, c) ? "red" : "white";
                            sw.WriteLine($"        <TD BGCOLOR=\"{color}\"></TD>");
                        }
                        sw.WriteLine("      </TR>");
                    }

                    sw.WriteLine("    </TABLE>");
                    sw.WriteLine("  >];");
                    sw.WriteLine("}");
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                // Asegúrate de que esta ruta sea correcta en tu PC
                startInfo.FileName = @"C:\Program Files\Graphviz\bin\dot.exe";
                startInfo.Arguments = $"-Tpng {nombreBase}.dot -o {nombreBase}.png";
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process.Start(startInfo);
            } catch (Exception ex) {
                Console.WriteLine("Aviso: No se pudo generar imagen: " + ex.Message);
            }
        }
    }
}