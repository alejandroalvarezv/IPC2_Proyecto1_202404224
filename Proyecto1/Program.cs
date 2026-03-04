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
                            Console.WriteLine("Memoria limpia.");
                            break;
                    }
                }
            }
        }

        // LÓGICA DE CARGA FIEL AL PDF
        static void CargarArchivo() 
        {
            Console.Write("Ingrese la ruta del archivo XML de entrada: ");
            string ruta = Console.ReadLine() ?? "";
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(ruta);
                
                // El PDF indica que los pacientes están en la raíz
                XmlNodeList listaXml = doc.GetElementsByTagName("paciente");
                foreach (XmlNode n in listaXml) {
                    Paciente p = new Paciente {
                        Nombre = n.SelectSingleNode("datospersonales/nombre")?.InnerText,
                        Edad = int.Parse(n.SelectSingleNode("datospersonales/edad")?.InnerText ?? "0"),
                        PeriodosMax = int.Parse(n.SelectSingleNode("periodos")?.InnerText ?? "0"),
                        M = int.Parse(n.SelectSingleNode("m")?.InnerText ?? "10")
                    };

                    // Lectura de la malla inicial según el PDF (atributos f y c)
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
                Console.WriteLine("Archivo de entrada cargado exitosamente.");
            } catch (Exception ex) { Console.WriteLine("Error al cargar XML: " + ex.Message); }
        }

        static void GenerarArchivoSalida()
{
    if (listaPacientes.Primero == null) {
        Console.WriteLine("No hay datos para exportar.");
        return;
    }

    Console.Write("Nombre para el archivo de resultados (ej: resultados.xml): ");
    string nombreArchivo = Console.ReadLine() ?? "resultados.xml";

    try {
        // Configuramos para que use UTF-8 y tenga indentación (como el ejemplo del PDF)
        XmlWriterSettings settings = new XmlWriterSettings { 
            Indent = true, 
            Encoding = System.Text.Encoding.UTF8 
        };

        using (XmlWriter writer = XmlWriter.Create(nombreArchivo, settings)) {
            writer.WriteStartDocument();
            
            // EL PDF PIDE QUE LA RAÍZ SEA <resultados>
            writer.WriteStartElement("resultados");

            NodoPaciente? aux = listaPacientes.Primero;
            while (aux != null) {
                Paciente? p = aux.Valor;
                if (p != null) {
                    writer.WriteStartElement("paciente");
                    
                    // Sección de datos personales (Nombre y Edad)
                    writer.WriteStartElement("datospersonales");
                    writer.WriteElementString("nombre", p.Nombre);
                    writer.WriteElementString("edad", p.Edad.ToString());
                    writer.WriteEndElement(); // Cierra datospersonales

                    // Resultado del análisis (Mortal, Grave o Leve)
                    writer.WriteElementString("resultado", p.Diagnostico);
                    
                    // Periodo donde se detectó el patrón e intervalo de repetición
                    writer.WriteElementString("n", p.n.ToString());
                    writer.WriteElementString("n1", p.n1.ToString());

                    writer.WriteEndElement(); // Cierra paciente
                }
                aux = aux.Siguiente;
            }

            writer.WriteEndElement(); // Cierra resultados
            writer.WriteEndDocument();
        }
        Console.WriteLine($"\n¡Éxito! Archivo '{nombreArchivo}' generado siguiendo el formato del PDF.");
    } catch (Exception ex) { 
        Console.WriteLine("Error al crear el XML de salida: " + ex.Message); 
    }
}

        static void SimularPasoAPaso() {
            if (pacienteSeleccionado == null) { Console.WriteLine("Seleccione un paciente."); return; }
            pacienteSeleccionado.MostrarEstadisticas(contadorPeriodo);
            pacienteSeleccionado.GenerarGrafica(contadorPeriodo);
            pacienteSeleccionado.Evolucionar();
            contadorPeriodo++;
            Console.WriteLine("Periodo procesado.");
        }

        static void SimularAutomatico() {
            if (pacienteSeleccionado == null) { Console.WriteLine("Seleccione un paciente."); return; }
            historialMallas.Limpiar();
            bool fin = false;
            for (int k = 0; k <= pacienteSeleccionado.PeriodosMax; k++) {
                string huella = pacienteSeleccionado.GenerarHuellaDigital();
                int rep = historialMallas.BuscarRepeticion(huella);
                if (rep != -1) {
                    pacienteSeleccionado.n = rep;
                    pacienteSeleccionado.n1 = k - rep;
                    pacienteSeleccionado.Diagnostico = (pacienteSeleccionado.n1 == 1) ? "Mortal" : "Grave";
                    fin = true; break;
                }
                historialMallas.Insertar(huella, k);
                pacienteSeleccionado.Evolucionar();
            }
            if (!fin) pacienteSeleccionado.Diagnostico = "Leve";
            Console.WriteLine($"Análisis completo: {pacienteSeleccionado.Diagnostico}");
        }

        static void ElegirPaciente() {
            if (listaPacientes.Primero == null) return;
            NodoPaciente? aux = listaPacientes.Primero;
            int i = 1;
            while (aux != null) {
                Console.WriteLine($"{i++}. {aux.Valor?.Nombre}");
                aux = aux.Siguiente;
            }
            if (int.TryParse(Console.ReadLine(), out int sel)) {
                aux = listaPacientes.Primero;
                for (int j = 1; j < sel && aux != null; j++) aux = aux.Siguiente;
                pacienteSeleccionado = aux?.Valor;
                contadorPeriodo = 0;
            }
        }
    }
}