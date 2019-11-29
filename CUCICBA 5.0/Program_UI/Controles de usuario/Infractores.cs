using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Program_UI.Controles_de_usuario
{
    public partial class Infractores : UserControl
    {
        public Infractores()
        {
            InitializeComponent();
        }

        String ruta = "";

        private void Button_Aceptar_Click(object sender, EventArgs e)
        {
            if (ruta != "")
            {
                using (StreamWriter fw = new StreamWriter("RutaInfractores.txt"))  //Creo si no existe el archivo de ruta y si ya existe se sobreescribe la linea con la nueva ruta
                {
                    fw.WriteLine(ruta);
                }

                MessageBox.Show("Se ha actualizado de manera satisfactoria la ruta deseada para sus archivos de infractores", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxRuta.Clear();
                Hide();
            }
            else
                MessageBox.Show("Debe seleccionar la carpeta en donde encontrará sus archivos de infractores", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Button_ElegirCarpeta_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ruta = folderBrowserDialog.SelectedPath;
            }

            textBoxRuta.Text = ruta;
        }

        private void Button_Cancelar_Click(object sender, EventArgs e)
        {
            textBoxRuta.Clear();
            Hide();
        }

        public void CargarTextBox()
        {
            try
            {
                using (StreamReader fw = new StreamReader("RutaInfractores.txt"))
                {
                    textBoxRuta.Text = fw.ReadLine();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
