using System;
using System.Xml; // Importante para la lectura del XML

namespace Proyecto1
{
    class Program 
    {
        // Lista global de pacientes
        static ListaEnlazada<Paciente> listaPacientes = new ListaEnlazada<Paciente>();

        static void Main(string[] args) 
        {
            int opcion = 0;
            while (opcion != 6) 
            {
                Console.WriteLine("\n--- MENU PRINCIPAL ---");
                Console.WriteLine("1. Cargar Archivo XML");
                Console.WriteLine("2. Elegir Paciente");
                Console.WriteLine("3. Ejecutar Simulación Paso a Paso");
                Console.WriteLine("4. Ejecutar Simulación Automática");
                Console.WriteLine("5. Limpiar Memoria");
                Console.WriteLine("6. Salir");
                Console.Write("Seleccione una opción: ");
                
                string entrada = Console.ReadLine() ?? "";
                if (int.TryParse(entrada, out opcion)) 
                {
                    switch (opcion) 
                    {
                        case 1: 
                            CargarArchivo();
                            break;
                        case 5:
                            listaPacientes.Limpiar();
                            Console.WriteLine("Memoria de pacientes limpia.");
                            break;
                        case 6:
                            Console.WriteLine("Saliendo del programa...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            }
        }

        // Método para cargar el XML (Opción 1)
        static void CargarArchivo()
        {
            Console.Write("Ingrese la ruta del archivo XML: ");
            string ruta = Console.ReadLine() ?? "";
            try 
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ruta);

                XmlNodeList pacientesXml = doc.GetElementsByTagName("paciente");

                foreach (XmlNode nodoPaciente in pacientesXml) 
                {
                    Paciente nuevoPaciente = new Paciente();
                    
                    nuevoPaciente.Nombre = nodoPaciente.SelectSingleNode("datospersonales/nombre")?.InnerText;
                    
                    string edadTexto = nodoPaciente.SelectSingleNode("datospersonales/edad")?.InnerText ?? "0";
                    nuevoPaciente.Edad = int.Parse(edadTexto);
                    
                    // El enunciado dice que M es el tamaño de la rejilla
                    string mTexto = nodoPaciente.SelectSingleNode("periodos")?.InnerText ?? "0";
                    nuevoPaciente.M = int.Parse(mTexto);

                    XmlNodeList celdasXml = nodoPaciente.SelectNodes("malla/celda");
                    if (celdasXml != null)
                    {
                        foreach (XmlNode nodoCelda in celdasXml) 
                        {
                            Celda nuevaCelda = new Celda {
                                Fila = int.Parse(nodoCelda.Attributes["f"]?.Value ?? "0"),
                                Columna = int.Parse(nodoCelda.Attributes["c"]?.Value ?? "0")
                            };
                            nuevoPaciente.CelulasContagiadas.Insertar(nuevaCelda);
                        }
                    }
                    listaPacientes.Insertar(nuevoPaciente);
                }
                Console.WriteLine("¡Archivo cargado con éxito!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error al cargar el archivo: " + ex.Message);
            }
        }
    }
}