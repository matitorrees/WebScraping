using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Clases;

namespace Program_UI
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SystemTrayApp());
        }
    }

    public unsafe class SystemTrayApp : ApplicationContext
    {
        private NotifyIcon trayIcon = new NotifyIcon();
        List<String> infractoresClarin = new List<String>();
        List<String> infractoresArgenProp = new List<String>();
        List<String> infractoresZonaProp = new List<String>();
        List<String> infractoresBuscaInmuebles = new List<String>();
        List<String> infractoresOLX = new List<String>();
        List<String> matriculados = new List<String>();
        List<Object> argsClarin = new List<object>();
        List<Object> argsArgenProp = new List<object>();
        List<Object> argsZonaProp = new List<object>();
        List<Object> argsBuscaInmuebles = new List<object>();
        List<Object> argsOLX = new List<object>();
        Boolean started = false;
        int counter = 0;


        public SystemTrayApp()
        {
            //Inicializacion
            trayIcon.Icon = Program_UI.Properties.Resources.search_flat1;
            trayIcon.Text = "CUCICBA";
            trayIcon.ContextMenu = new ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(Exit_button)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Start", new EventHandler(Start_button)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Configuration", new EventHandler(Config_button)));
            trayIcon.Visible = true;
            ThreadPool.SetMinThreads(201, 101);
            argsClarin.Add("");//Cargo el primer elemento, despues se reemplaza con el link de la pagina
            argsClarin.Add(0);//Este es un contador de la cantidad de threads activas que buscan en la pagina correspondiente
            argsClarin.Add(infractoresClarin);//La lista para guardar los infractores
            argsClarin.Add(0);//Contador de paginas revisadas
            argsClarin.Add(0);//Contador de fallas
            //Repite el patron//
            argsArgenProp.Add("");
            argsArgenProp.Add(0);
            argsArgenProp.Add(infractoresArgenProp);
            argsArgenProp.Add(0);
            argsArgenProp.Add(0);
            argsZonaProp.Add("");
            argsZonaProp.Add(0);
            argsZonaProp.Add(infractoresZonaProp);
            argsZonaProp.Add(0);
            argsZonaProp.Add(0);
            argsBuscaInmuebles.Add("");
            argsBuscaInmuebles.Add(0);
            argsBuscaInmuebles.Add(infractoresBuscaInmuebles);
            argsBuscaInmuebles.Add(0);
            argsBuscaInmuebles.Add(0);
            /////////////////////
            argsOLX.Add("");
            argsOLX.Add(0);
            argsOLX.Add(infractoresOLX);
            argsOLX.Add(0);


            ///////////////////// Sección en donde se cargan los matriculados a la lista de matriculados para futura comparación con los nombres extraidos
            ///////////////////// Si no se configuró la ruta en donde se encuentra el archivo no se cargará nada a la lista ni se creará otro archivo

            String rutaMatriculados;

            try
            {
                using (StreamReader fw = new StreamReader("RutaMatriculados.txt"))
                {
                    rutaMatriculados = fw.ReadLine();
                }

                if (rutaMatriculados == null)
                {
                    MessageBox.Show("Debe configurar la carpeta en donde se encontrará el archivo de matriculados", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    try
                    {
                        foreach (String nombre in File.ReadAllLines(rutaMatriculados + "\\matriculados.txt"))
                        {
                            //MessageBox.Show(nombre);
                            matriculados.Add(nombre);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("El archivo matriculados no se encuentra en la carpeta especificada en la configuración de la ruta. Por favor vuelva a configurar la carpeta en donde se encuentra el archivo o coloque el archivo en la carpeta que especificó anteriormente", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Debe configurar la carpeta en donde se encontrará el archivo de matriculados", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            /////////////////////////             
        }


        public unsafe void Exit_button(object sender, EventArgs ev)
        {
            Application.Exit();
        }

        public unsafe void Config_button(object sender, EventArgs ev)
        {
            Form form = new Form();
            Configuración f = new Configuración();
            f.Show();
        }


        public unsafe void Start_button(object sender, EventArgs ev)
        {
            Thread thread;

            if (started)
            {
                started = false;
                trayIcon.Icon = Program_UI.Properties.Resources.cross_flat;
                while ((int)argsOLX[1] != 0 || (int)argsClarin[1] != 0 || (int)argsArgenProp[1] != 0 || (int)argsZonaProp[1] != 0 || (int)argsBuscaInmuebles[1] != 0) { }// Con esto controlo si las hebras de busqueda terminaron, Comparo por el contador de hebras activas.
                trayIcon.Icon = Program_UI.Properties.Resources.search_flat1;
                trayIcon.ContextMenu.MenuItems[1].Text = "Start";
                counter = 0;
                Guardar();//Termino guardando en los txt correspondientes

            }
            else
            {
                started = true;
                thread = new Thread(new ThreadStart(StartExtraction));
                thread.Start();
                trayIcon.Icon = Program_UI.Properties.Resources.Icon1;
                trayIcon.ContextMenu.MenuItems[1].Text = "Stop";

            }
        }


        public void Guardar()
        {
            String rutaInfractores;

            try
            {
                using (StreamReader fw = new StreamReader("RutaInfractores.txt"))
                {
                    rutaInfractores = fw.ReadLine();
                }

                if (rutaInfractores == null)
                {
                    MessageBox.Show("Debe configurar la carpeta en donde se encontraran los archivos de infractores", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Los archivos se crearan en la carpeta donde se encuentra el ejecutable", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    File.WriteAllLines("infractoresClarin.txt", ((List<String>)((List<object>)argsClarin)[2]));
                    File.WriteAllLines("infractoresArgenProp.txt", ((List<String>)((List<object>)argsArgenProp)[2]));
                    File.WriteAllLines("infractoresZonaProp.txt", ((List<String>)((List<object>)argsZonaProp)[2]));
                    File.WriteAllLines("infractoresBuscaInmuebles.txt", ((List<String>)((List<object>)argsBuscaInmuebles)[2]));
                    File.WriteAllLines("infractoresOLX.txt", ((List<String>)((List<object>)argsOLX)[2]));
                    //foreach (string s in ((List<String>)((List<object>)argsClarin)[2]))
                    //    Console.WriteLine(s);
                    ((List<String>)((List<object>)argsArgenProp)[2]).Clear();
                    ((List<String>)((List<object>)argsClarin)[2]).Clear();
                    ((List<String>)((List<object>)argsZonaProp)[2]).Clear();
                    ((List<String>)((List<object>)argsBuscaInmuebles)[2]).Clear();
                    ((List<String>)((List<object>)argsOLX)[2]).Clear();


                }
                else
                {
                    File.WriteAllLines(rutaInfractores + "\\infractoresClarin.txt", ((List<String>)((List<object>)argsClarin)[2]));
                    File.WriteAllLines(rutaInfractores + "\\infractoresArgenProp.txt", ((List<String>)((List<object>)argsArgenProp)[2]));
                    File.WriteAllLines(rutaInfractores + "\\infractoresZonaProp.txt", ((List<String>)((List<object>)argsZonaProp)[2]));
                    File.WriteAllLines(rutaInfractores + "\\infractoresBuscaInmuebles.txt", ((List<String>)((List<object>)argsBuscaInmuebles)[2]));
                    File.WriteAllLines(rutaInfractores + "\\infractoresOLX.txt", ((List<String>)((List<object>)argsOLX)[2]));
                    //foreach (string s in ((List<String>)((List<object>)argsClarin)[2]))
                    //    Console.WriteLine(s);
                    ((List<String>)((List<object>)argsArgenProp)[2]).Clear();
                    ((List<String>)((List<object>)argsClarin)[2]).Clear();
                    ((List<String>)((List<object>)argsZonaProp)[2]).Clear();
                    ((List<String>)((List<object>)argsBuscaInmuebles)[2]).Clear();
                    ((List<String>)((List<object>)argsOLX)[2]).Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Debe configurar la carpeta en donde se encontraran los archivos de infractores", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Los archivos se crearan en la carpeta donde se encuentra el ejecutable", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                File.WriteAllLines("infractoresClarin.txt", ((List<String>)((List<object>)argsClarin)[2]));
                File.WriteAllLines("infractoresArgenProp.txt", ((List<String>)((List<object>)argsArgenProp)[2]));
                File.WriteAllLines("infractoresZonaProp.txt", ((List<String>)((List<object>)argsZonaProp)[2]));
                File.WriteAllLines("infractoresBuscaInmuebles.txt", ((List<String>)((List<object>)argsBuscaInmuebles)[2]));
                File.WriteAllLines("infractoresOLX.txt", ((List<String>)((List<object>)argsOLX)[2]));
                //foreach (string s in ((List<String>)((List<object>)argsClarin)[2]))
                //    Console.WriteLine(s);
                ((List<String>)((List<object>)argsArgenProp)[2]).Clear();
                ((List<String>)((List<object>)argsClarin)[2]).Clear();
                ((List<String>)((List<object>)argsZonaProp)[2]).Clear();
                ((List<String>)((List<object>)argsBuscaInmuebles)[2]).Clear();
                ((List<String>)((List<object>)argsOLX)[2]).Clear();
            }
        }


        public void StartExtraction()
        {
            Thread thread;
            ExtractorClarin eClarin;
            ExtractorArgenProp eArgenProp;
            ExtractorZonaProp eZonaProp;
            ExtractorBuscaInmuebles eBuscaInmuebles;
            ExtractorOLX eOLX;

            int i = 1;
            while (started)
            {
                eClarin = new ExtractorClarin(matriculados);
                eArgenProp = new ExtractorArgenProp(matriculados);
                eZonaProp = new ExtractorZonaProp(matriculados);
                eBuscaInmuebles = new ExtractorBuscaInmuebles(matriculados);
                eOLX = new ExtractorOLX(matriculados);


                while (((int)argsClarin[1] + (int)argsArgenProp[1]) + /*(int)argsZonaProp[1] +*/ (int)argsBuscaInmuebles[1] /*+ (int)argsOLX[1]*/ > 100) { Thread.Sleep(1000); }//Con esto controlo de no pasarme de "n" cantidad de hebras
                lock (this)
                {
                    argsClarin[1] = (int)argsClarin[1] + 1;
                    argsArgenProp[1] = (int)argsArgenProp[1] + 1;
                    //argsZonaProp[1] = (int)argsZonaProp[1] + 1;
                    argsBuscaInmuebles[1] = (int)argsBuscaInmuebles[1] + 1;
                    //argsOLX[1] = (int)argsOLX[1] + 1;
                }
                //Cargo los args a enviar a cada hebra//
                argsClarin[0] = "https://www.inmuebles.clarin.com/inmuebles-venta-localidad-capital-federal-pagina-" + i;
                argsArgenProp[0] = "https://www.argenprop.com/inmuebles-localidad-capital-federal-pagina-" + i;
                argsBuscaInmuebles[0] = "https://www.buscainmueble.com/inmuebles-pais-argentina-pagina-" + i;
                //argsZonaProp[0] = "https://www.zonaprop.com.ar/inmuebles-pagina-" + i + ".html";
                //argsOLX[0] = "https://capitalfederal.olx.com.ar/inmuebles-y-propiedades-cat-16-p-" + i;

                //Lanzo las hebras de busqueda//
                thread = new Thread(new ParameterizedThreadStart(eClarin.Extract));
                thread.Priority = ThreadPriority.Highest;
                thread.Start(argsClarin);
                Thread.Sleep(50);

                thread = new Thread(new ParameterizedThreadStart(eArgenProp.Extract));
                thread.Priority = ThreadPriority.Highest;
                thread.Start(argsArgenProp);
                Thread.Sleep(50);

                //thread = new Thread(new ParameterizedThreadStart(eZonaProp.Extract));
                //thread.Priority = ThreadPriority.Highest;
                //thread.Start(argsZonaProp);
                //Thread.Sleep(50);

                thread = new Thread(new ParameterizedThreadStart(eBuscaInmuebles.Extract));
                thread.Priority = ThreadPriority.Highest;
                thread.Start(argsBuscaInmuebles);
                Thread.Sleep(50);

                //thread = new Thread(new ParameterizedThreadStart(eOLX.Extract));
                //thread.Priority = ThreadPriority.Highest;
                //thread.Start(argsOLX);
                //Thread.Sleep(50);


                i++;

                lock (this)

                {
                    if ((int)argsClarin[4] + (int)argsArgenProp[4] + /*(int)argsZonaProp[4] +*/ (int)argsBuscaInmuebles[4] > 3)
                    {
                        Thread.Sleep(240000);
                        argsClarin[4] = 0;
                        argsArgenProp[4] = 0;
                        argsZonaProp[4] = 0;
                        argsBuscaInmuebles[4] = 0;
                    }
                    counter = (int)argsClarin[3] + (int)argsArgenProp[3] /*+ (int)argsZonaProp[3]*/ + (int)argsBuscaInmuebles[3] /*+(int) argsOLX[3]*/;
                }

                trayIcon.ContextMenu.MenuItems[1].Text = "Stop(" + counter + " Páginas analizadas)";
            }
        }
    }
}
