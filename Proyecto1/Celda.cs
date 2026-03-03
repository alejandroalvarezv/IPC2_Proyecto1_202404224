using System;
using System.IO;
using System.Diagnostics;

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

        // Variables añadidas para el Análisis del Release 3
        public string Diagnostico { get; set; } = "Leve";
        public int n { get; set; } = 0;  // Periodo donde inicia el patrón
        public int n1 { get; set; } = 0; // Tiempo de repetición

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
            // Se usa System.Text.StringBuilder para optimizar la memoria al crear la huella
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
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

        public void GenerarGrafica(int periodo) {
            string nombreBase = $"Paciente_{Nombre}_T{periodo}";
            try {
                using (StreamWriter sw = new StreamWriter(nombreBase + ".dot")) {
                    sw.WriteLine("digraph G {");
                    sw.WriteLine("  node [shape=box, label=\"\", width=0.3, height=0.3];");
                    sw.WriteLine($"  label=\"Paciente: {Nombre} - Periodo: {periodo}\";");
                    for (int f = 1; f <= M; f++) {
                        for (int c = 1; c <= M; c++) {
                            string color = EstaContagiada(f, c) ? "red" : "white";
                            sw.WriteLine($"  n_{f}_{c} [style=filled, fillcolor={color}];");
                        }
                    }
                    sw.WriteLine("}");
                }
                
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Program Files\Graphviz\bin\dot.exe";
                startInfo.Arguments = $"-Tpng {nombreBase}.dot -o {nombreBase}.png";
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process.Start(startInfo);
            } catch (Exception) {
                Console.WriteLine("Aviso: No se pudo generar PNG.");
            }
        }
    }
}