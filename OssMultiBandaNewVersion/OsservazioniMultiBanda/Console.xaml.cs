using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OssMultiBanda
{
    /// <summary>
    /// Logica di interazione per Console.xaml
    /// </summary>
    public partial class Console : Window
    {
        public Console()
        {
            InitializeComponent();
        }

        public void WriteLine(string text)
        {
            textbox_console.Text = text + Environment.NewLine;
        }
    }
}
