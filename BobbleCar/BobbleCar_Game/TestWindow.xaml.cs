using MahApps.Metro.Controls;
using SimpleGraphicsLib;
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
using System.Windows.Shapes;

namespace BobbleCar_Game
{
    /// <summary>
    /// Interaktionslogik für TestWindow.xaml
    /// </summary>
    public partial class TestWindow : MetroWindow
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            // Objekte erstellen
            IGFXObject obj1 = new SpriteObject("Test Sprite");
            //IGFXObject obj2 = new CarObject("Test Auto");  // erstellt ParticleSystem. passt nicht zu neuer Baumstruktur
            IGFXObject obj2 = new SpriteObjectElastic("Fussball");
            IGFXObjComposition comp2 = new GFXComposition("Blumenbeet");
            comp2.AddObject(new SpriteObject("Sehr grüne Pflanze"));
            comp2.AddObject(new SpriteObject("Maiglöckchen"));

            IGFXObjComposition comp1 = new GFXComposition("Zusammengebautes Object");
            comp1.AddObject(new SpriteObject("Teil 1"));
            comp1.AddObject(new SpriteObject("Teil 2"));
            comp1.AddObject(comp2);
            comp1.AddObject(new SpriteObject("Teil 3"));

            MainGFX.AddObject(obj1);
            MainGFX.AddObject(obj2);
            MainGFX.AddObject(comp1);
            

            // Objekt-Baum listen
            lstTree.ItemsSource = MainGFX.ToList();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
