using System;
using System.Xml;

namespace Proyecto1
{
    class Program 
    {
        static ListaEnlazada<Paciente> listaPacientes = new ListaEnlazada<Paciente>();
        static Paciente? pacienteSeleccionado = null;

        static void Main(string[] args) 
        {
            int opcion = 0;
            while (opcion != 4) 
            {
                Console.WriteLine("\n--- MENU PRINCIPAL ---");
                Console.WriteLine("1. Cargar Archivo XML");
                Console.WriteLine("2. Elegir Paciente");
                Console.WriteLine("3. Limpiar Memoria");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                
                string entrada = Console.ReadLine() ?? "";
                if (int.TryParse(entrada, out opcion)) 
                {
                    switch (opcion) 
                    {
                        case 1: CargarArchivo(); break;
                        case 2: ElegirPaciente(); break;
                        case 3:
                            listaPacientes.Limpiar();
                            pacienteSeleccionado = null;
                            Console.WriteLine("Memoria limpia.");
                            break;
                        case 4: Console.WriteLine("Saliendo..."); break;
                    }
                }
            }
        }

        static void CargarArchivo() 
        {
            Console.Write("Ruta XML: ");
            string ruta = Console.ReadLine() ?? "";
            try 
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ruta);
                XmlNodeList listaXml = doc.GetElementsByTagName("paciente");
                foreach (XmlNode n in listaXml) 
                {
                    Paciente p = new Paciente();
                    p.Nombre = n.SelectSingleNode("datospersonales/nombre")?.InnerText;
                    p.Edad = int.Parse(n.SelectSingleNode("datospersonales/edad")?.InnerText ?? "0");
                    p.PeriodosMax = int.Parse(n.SelectSingleNode("periodos")?.InnerText ?? "0");
                    p.M = int.Parse(n.SelectSingleNode("m")?.InnerText ?? "10"); 
                    
                    XmlNodeList celdas = n.SelectNodes("malla/celda");
                    foreach (XmlNode c in celdas!) 
                    {
                        p.CelulasContagiadas.Insertar(new Celda {
                            Fila = int.Parse(c.Attributes!["f"]!.Value),
                            Columna = int.Parse(c.Attributes!["c"]!.Value)
                        });
                    }
                    listaPacientes.Insertar(p);
                }
                Console.WriteLine("Carga exitosa.");
            } 
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        static void ElegirPaciente() 
        {
            if (listaPacientes.Primero == null) 
            {
                Console.WriteLine("No hay pacientes cargados.");
                return;
            }
            Nodo<Paciente>? aux = listaPacientes.Primero;
            int i = 1;
            while (aux != null) 
            {
                Console.WriteLine($"{i}. {aux.Valor?.Nombre}");
                aux = aux.Siguiente; i++;
            }
            if (int.TryParse(Console.ReadLine(), out int sel)) 
            {
                aux = listaPacientes.Primero;
                for (int j = 1; j < sel && aux != null; j++) aux = aux.Siguiente;
                pacienteSeleccionado = aux?.Valor;
                Console.WriteLine($"Paciente {pacienteSeleccionado?.Nombre} seleccionado.");
            }
        }
    }
}