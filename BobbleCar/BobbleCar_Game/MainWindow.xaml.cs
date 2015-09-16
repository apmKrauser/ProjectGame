using BobbleCar;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

namespace BobbleCar_Game
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // GameWindow GW = new GameWindow();
           // GW.Show();
           //// this.Close();
           // this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameWindow GW = new GameWindow("LevelOne.xml");
            GW.Show();
            // this.Close();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GameWindow GW = new GameWindow("mplay_test.xml");
            GW.Show();
            // this.Close();
            this.Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GameDesigner GW = new GameDesigner();
            GW.Show();
            // this.Close();
            this.Hide();
        }

    }
}
