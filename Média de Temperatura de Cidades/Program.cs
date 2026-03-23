using System;
using System.Windows.Forms;

namespace AppCidades
{
    static class Program
    {
        /// <summary>
        /// O ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread] // Necessário para que a interface gráfica funcione corretamente no Windows
        static void Main()
        {
            // Habilita os estilos visuais modernos do Windows (botões arredondados, cores, etc)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia o formulário principal que criamos anteriormente
            Application.Run(new MainForm());
        }
    }
}