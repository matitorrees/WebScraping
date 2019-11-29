using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Program_UI
{
    public partial class Configuración : Form
    {
        public Configuración()
        {
            InitializeComponent();
            matriculados1.Hide();
            infractores1.Hide();
        }


        private void Button_Matriculados_Click(object sender, EventArgs e)
        {
            matriculados1.CargarTextBox();
            matriculados1.Show();
            matriculados1.BringToFront();
            infractores1.Hide();
        }

        private void Button_Infractores_Click(object sender, EventArgs e)
        {
            infractores1.CargarTextBox();
            infractores1.Show();
            infractores1.BringToFront();
            matriculados1.Hide();
        }





       
        /////////////////////////////////////////////////////////////////////
       


        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]

        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void Panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BotonCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BotonMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        
    }
}
