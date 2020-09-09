using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kursovaya_rabota_3_semestr
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Gif picture = new Gif();
            string writePath = @"simple.gif";
            byte[] file = picture.TestGenerate();
            textBox.Text =picture.text;

            try
            {
                using (FileStream fstream = new FileStream(writePath, FileMode.Truncate))
                {
                    await fstream.WriteAsync(file, 0, file.Length);

                  fstream.Close();
                }
  
            }
            catch (Exception )
            {

            }
        }
    }    
}
