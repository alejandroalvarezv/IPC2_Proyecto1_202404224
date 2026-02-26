using System;
using System.Xml;

namespace Proyecto1
{
    class Program 
    {
        static ListaPaciente listaPacientes = new ListaPaciente();
        static Paciente? pacienteSeleccionado = null;
        static int contadorPeriodo = 0;

        static void Main(string[] args) 
        {
            int opcion = 0;
            while (opcion != 6) 
            {
                Console.WriteLine("\n--- MENU PRINCIPAL ---");
                Console.WriteLine("1. Cargar Archivo XML");
                Console.WriteLine("2. Elegir Paciente");
                Console.WriteLine("3. Simulación paso a paso");
                Console.WriteLine("4. Simulación Automática");
                Console.WriteLine("5. Limpiar Memoria");
                Console.WriteLine("6. Salir");
                Console.Write("Seleccione una opción: ");
                
                if (int.TryParse(Console.ReadLine(), out opcion)) 
                {
                    switch (opcion) 
                    {
                        case 1: CargarArchivo(); break;
                        case 2: ElegirPaciente(); break;
                        case 3:
                            if (pacienteSeleccionado != null) {
                                pacienteSeleccionado.GenerarGrafica(contadorPeriodo);
                                pacienteSeleccionado.Evolucionar();
                                contadorPeriodo++;
                                Console.WriteLine($"Periodo {contadorPeriodo} completado.");
                            } else Console.WriteLine("Seleccione un paciente primero.");
                            break;
                        case 4:
                            if (pacienteSeleccionado != null) {
                                for(int k = 0; k < pacienteSeleccionado.PeriodosMax; k++) {
                                    pacienteSeleccionado.GenerarGrafica(k);
                                    pacienteSeleccionado.Evolucionar();
                                }
                                Console.WriteLine("Simulación terminada.");
                            } else Console.WriteLine("Seleccione un paciente primero.");
                            break;
                        case 5:
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
            Console.Write("Ruta XML: ");
            string ruta = Console.ReadLine() ?? "";
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(ruta);
                XmlNodeList listaXml = doc.GetElementsByTagName("paciente");
                foreach (XmlNode n in listaXml) {
                    Paciente p = new Paciente {
                        Nombre = n.SelectSingleNode("datospersonales/nombre")?.InnerText,
                        Edad = int.Parse(n.SelectSingleNode("datospersonales/edad")?.InnerText ?? "0"),
                        PeriodosMax = int.Parse(n.SelectSingleNode("periodos")?.InnerText ?? "0"),
                        M = int.Parse(n.SelectSingleNode("m")?.InnerText ?? "10")
                    };
                    XmlNodeList? celdas = n.SelectNodes("malla/celda");
                    if (celdas != null) {
                        foreach (XmlNode c in celdas) {
                            p.CelulasContagiadas.Insertar(new Celda {
                                Fila = int.Parse(c.Attributes?["f"]?.Value ?? "0"),
                                Columna = int.Parse(c.Attributes?["c"]?.Value ?? "0")
                            });
                        }
                    }
                    listaPacientes.Insertar(p);
                }
                Console.WriteLine("Carga exitosa.");
            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
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
            if (int.TryParse(Console.ReadLine(), out int sel)) {
                aux = listaPacientes.Primero;
                for (int j = 1; j < sel && aux != null; j++) aux = aux.Siguiente;
                pacienteSeleccionado = aux?.Valor;
                contadorPeriodo = 0;
                Console.WriteLine($"Paciente {pacienteSeleccionado?.Nombre} seleccionado.");
            }
        }
    }
}