using System;
using System.Xml;

namespace Proyecto1
{
    class Program 
    {
        static ListaPaciente listaPacientes = new ListaPaciente();
        static Paciente? pacienteSeleccionado = null;
        static int contadorPeriodo = 0;
        static ListaHistorial historialMallas = new ListaHistorial();

        static void Main(string[] args) 
        {
            int opcion = 0;
            while (opcion != 7) 
            {
                Console.WriteLine("\n--- MENU PRINCIPAL ---");
                Console.WriteLine("1. Cargar Archivo XML (Entrada)");
                Console.WriteLine("2. Elegir Paciente");
                Console.WriteLine("3. Simulación paso a paso");
                Console.WriteLine("4. Simulación Automática (Diagnóstico)");
                Console.WriteLine("5. Generar Archivo XML de Salida");
                Console.WriteLine("6. Limpiar Memoria");
                Console.WriteLine("7. Salir");
                Console.Write("Seleccione una opción: ");
                
                if (int.TryParse(Console.ReadLine(), out opcion)) 
                {
                    switch (opcion) 
                    {
                        case 1: CargarArchivo(); break;
                        case 2: ElegirPaciente(); break;
                        case 3: SimularPasoAPaso(); break;
                        case 4: SimularAutomatico(); break;
                        case 5: GenerarArchivoSalida(); break;
                        case 6:
                            listaPacientes.Limpiar();
                            pacienteSeleccionado = null;
                            contadorPeriodo = 0;
                            Console.WriteLine("Memoria limpia.");
                            break;
                    }
                }
            }
        }

        static void CargarArchivo() 
{
    Console.Write("Ingrese la ruta del archivo XML: ");
    string ruta = Console.ReadLine() ?? "";
    if (!File.Exists(ruta)) {
        Console.WriteLine("El archivo no existe.");
        return;
    }

    try {
        using (StreamReader sr = new StreamReader(ruta)) {
            string contenido = sr.ReadToEnd();
            string[] pacientesRaw = contenido.Split(new string[] { "<paciente>" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < pacientesRaw.Length; i++) {
                string pData = pacientesRaw[i].Split(new string[] { "</paciente>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                
                Paciente p = new Paciente();
                p.Nombre = ExtraerValor(pData, "<nombre>", "</nombre>");
                p.Edad = int.Parse(ExtraerValor(pData, "<edad>", "</edad>") ?? "0");
                p.PeriodosMax = int.Parse(ExtraerValor(pData, "<periodos>", "</periodos>") ?? "0");
                p.M = int.Parse(ExtraerValor(pData, "<m>", "</m>") ?? "10");

                string[] celdasRaw = pData.Split(new string[] { "<celda" }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j < celdasRaw.Length; j++) {
                    string cData = celdasRaw[j].Split('>')[0];
                    int f = int.Parse(ExtraerAtributo(cData, "f="));
                    int c = int.Parse(ExtraerAtributo(cData, "c="));
                    p.CelulasContagiadas.Insertar(new Celda { Fila = f, Columna = c });
                }
                listaPacientes.Insertar(p);
            }
        }
        Console.WriteLine("Archivo cargado con éxito (Lector Manual).");
    } catch (Exception ex) {
        Console.WriteLine("Error en lectura manual: " + ex.Message);
    }
}

static string? ExtraerValor(string data, string tagInicio, string tagFin) {
    if (!data.Contains(tagInicio)) return null;
    int inicio = data.IndexOf(tagInicio) + tagInicio.Length;
    int fin = data.IndexOf(tagFin, inicio);
    return data.Substring(inicio, fin - inicio).Trim();
}

static string ExtraerAtributo(string data, string attr) {
    int inicio = data.IndexOf(attr) + attr.Length + 1;
    int fin = data.IndexOf('\"', inicio);
    return data.Substring(inicio, fin - inicio);
}

        static void ElegirPaciente() 
        {
            if (listaPacientes.Primero == null) {
                Console.WriteLine("No hay pacientes cargados.");
                return;
            }
            NodoPaciente? aux = listaPacientes.Primero;
            int i = 1;
            while (aux != null) {
                Console.WriteLine($"{i}. {aux.Valor?.Nombre}");
                aux = aux.Siguiente; i++;
            }
            Console.Write("Seleccione el número de paciente: ");
            if (int.TryParse(Console.ReadLine(), out int sel)) {
                aux = listaPacientes.Primero;
                for (int j = 1; j < sel && aux != null; j++) aux = aux.Siguiente;
                pacienteSeleccionado = aux?.Valor;
                contadorPeriodo = 0;
                Console.WriteLine($"Paciente {pacienteSeleccionado?.Nombre} seleccionado.");
            }
        }

        static void SimularPasoAPaso() 
        {
            if (pacienteSeleccionado == null) {
                Console.WriteLine("Debe elegir un paciente primero.");
                return;
            }
            pacienteSeleccionado.MostrarEstadisticas(contadorPeriodo);
            pacienteSeleccionado.GenerarGrafica(contadorPeriodo);
            pacienteSeleccionado.Evolucionar();
            contadorPeriodo++;
            Console.WriteLine($"Periodo {contadorPeriodo} completado y graficado.");
        }

        static void SimularAutomatico() 
        {
            if (pacienteSeleccionado == null) {
                Console.WriteLine("Debe elegir un paciente primero.");
                return;
            }

            historialMallas.Limpiar();
            bool patronYaRegistrado = false;

            for (int k = 0; k <= pacienteSeleccionado.PeriodosMax; k++) 
            {
                pacienteSeleccionado.MostrarEstadisticas(k);
                pacienteSeleccionado.GenerarGrafica(k);
                
                string huellaActual = pacienteSeleccionado.GenerarHuellaDigital();
                int periodoAnterior = historialMallas.BuscarRepeticion(huellaActual);

                if (periodoAnterior != -1 && !patronYaRegistrado) 
                {
                    pacienteSeleccionado.n = periodoAnterior; 
                    pacienteSeleccionado.n1 = k - periodoAnterior; 
                    
                    pacienteSeleccionado.Diagnostico = (pacienteSeleccionado.n1 == 1) ? "MORTAL" : "GRAVE";

                    Console.WriteLine($"\n¡PATRÓN DETECTADO en periodo {k}!");
                    Console.WriteLine($"Se guardó: {pacienteSeleccionado.Diagnostico} (n={pacienteSeleccionado.n}, n1={pacienteSeleccionado.n1})");
                    Console.WriteLine("Continuando simulación hasta el límite máximo...");
                    
                    patronYaRegistrado = true; 
                }

                historialMallas.Insertar(huellaActual, k);
                
                if (k < pacienteSeleccionado.PeriodosMax) {
                    pacienteSeleccionado.Evolucionar();
                }
            }

            if (!patronYaRegistrado) {
                pacienteSeleccionado.Diagnostico = "LEVE";
                pacienteSeleccionado.n = 0;
                pacienteSeleccionado.n1 = 0;
                Console.WriteLine("\nResultado final: La enfermedad es LEVE.");
            } else {
                Console.WriteLine($"\nSimulación completa. Diagnóstico final: {pacienteSeleccionado.Diagnostico}");
            }
        }

        static void GenerarArchivoSalida()
{
    if (listaPacientes.Primero == null) {
        Console.WriteLine("No hay datos para exportar.");
        return;
    }

    Console.Write("Nombre para el archivo de salida: ");
    string nombreArchivo = Console.ReadLine() ?? "resultados.xml";

    try {
        using (StreamWriter sw = new StreamWriter(nombreArchivo)) {
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sw.WriteLine("<pacientes>");

            NodoPaciente? aux = listaPacientes.Primero;
            while (aux != null) {
                Paciente? p = aux.Valor;
                if (p != null) {
                    sw.WriteLine("    <paciente>");
                    sw.WriteLine("        <datospersonales>");
                    sw.WriteLine($"            <nombre>{p.Nombre}</nombre>");
                    sw.WriteLine($"            <edad>{p.Edad}</edad>");
                    sw.WriteLine("        </datospersonales>");
                    sw.WriteLine($"        <periodos>{p.PeriodosMax}</periodos>");
                    sw.WriteLine($"        <m>{p.M}</m>");
                    sw.WriteLine($"        <resultado>{p.Diagnostico}</resultado>");
                    
                    if (p.Diagnostico != "LEVE") {
                        sw.WriteLine($"        <n>{p.n}</n>");
                        sw.WriteLine($"        <n1>{p.n1}</n1>");
                    }
                    
                    sw.WriteLine("    </paciente>");
                }
                aux = aux.Siguiente;
            }

            sw.WriteLine("</pacientes>");
        }
        Console.WriteLine($"\n¡Listo! Archivo '{nombreArchivo}' generado con éxito.");
    } catch (Exception ex) { 
        Console.WriteLine("Error al generar salida: " + ex.Message); 
    }
}
    }
}