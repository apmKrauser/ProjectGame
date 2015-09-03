using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace ZweiachsMofa
{
    /// <summary>
    /// Interaktionslogik für PropertyInspect.xaml
    /// </summary>
    public partial class PropertyInspect : MetroWindow
    {
        IGFXObject GObj;

//        public ObservableCollection<PropertyGridSet> PList = new ObservableCollection<PropertyGridSet>();
        public List<PropertyGridSet> PList = new List<PropertyGridSet>();
        //public BindingList<PropertyGridSet> PList = new BindingList<PropertyGridSet>();

        public PropertyInspect()
        {
            InitializeComponent();
        }

        public PropertyInspect(IGFXObject obj)
        {
            GObj = obj;
            InitializeComponent();
            PropertyGrid.ItemsSource = PList;
            ReadProperties();
            PropertyGrid.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ReadProperties();
            PropertyGrid.Items.Refresh();
            var s = new Vector(0, 5);
            var t = s.ToString(CultureInfo.InvariantCulture);
            var t2 = s.ToString();
            var o = Vector.Parse(t);
            double d = 5.33;
            var d1 = d.ToString();

            var x = double.Parse(d1);
            var x2 = double.Parse(d1, CultureInfo.InvariantCulture);
            int ii;
        }

        private void ReadProperties()
        {
            PList.Clear();
            //Assembly.GetExecutingAssembly();
            var typ = GObj.GetType();
            foreach (var item in typ.GetProperties())
            {
                object v = item.GetValue(GObj);
                var t = item.PropertyType;
                if (((v is String) ||
                   (v is Double) ||
                   (v is int) ||
                   (v is bool) ||
                   (v is Vector))
                    && item.CanWrite)
                {
                    PList.Add(new PropertyGridSet { Name = item.Name, Type = t.ToString(), ValueObj = v });
                }
            }

        }

        private void UpdateProperty(PropertyGridSet propSet)
        {
            var typ = GObj.GetType();
            var prop = typ.GetProperty(propSet.Name);
            prop.SetValue(GObj, propSet.ValueObj);
        }

        private void PropertyGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
        }


        private void DGEditStyleTmpl_LostFocus(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("lost");
            try
            {
                PropertyGridSet item = PropertyGrid.SelectedItem as PropertyGridSet;
                if (sender is TextBox)
                {
                    string txt = (sender as TextBox).Text;
                    item.SetValue(txt);
                }
                else if (sender is CheckBox)
                {
                    var b = sender as CheckBox;
                    item.SetValue(b.IsChecked);
                }
                UpdateProperty(item);
            }
            catch (Exception ex)
            {
               // e.Cancel = true;
            }
            finally
            {
                PropertyGrid.Items.Refresh();
            }
        }


    }
}
