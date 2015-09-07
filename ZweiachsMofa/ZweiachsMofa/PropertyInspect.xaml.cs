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
        IPropertyInspectable GObj;

//        public ObservableCollection<PropertyGridSet> PList = new ObservableCollection<PropertyGridSet>();
        public List<PropertyGridItem> PList = new List<PropertyGridItem>();
        //public BindingList<PropertyGridSet> PList = new BindingList<PropertyGridSet>();


        public Visibility ShowEditAnimators
        {
            get 
            { 
                return (GObj is SpriteObject) ? Visibility.Visible : Visibility.Hidden ; 
            }
            set { }
        }
        

        public PropertyInspect()
        {
            InitializeComponent();
        }

        public PropertyInspect(IPropertyInspectable obj)
        {
            GObj = obj;
            InitializeComponent();
            PropertyGrid.ItemsSource = PList;
            ReadProperties();
            PropertyGrid.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditAnimators edani = new EditAnimators(GObj);
            try
            {
                edani.Show();
            }
            catch (Exception)
            {
            }
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
                   (v is Key) ||
                   (v is Vector)) ||
                   (v is IPropertyInspectable)
                    && item.CanWrite)
                {
                    PList.Add(new PropertyGridItem { Name = item.Name, Type = t.ToString(), ValueObj = v });
                }
            }

        }

        private void UpdateProperty(PropertyGridItem propItem)
        {
            var typ = GObj.GetType();
            var prop = typ.GetProperty(propItem.Name);
            prop.SetValue(GObj, propItem.ValueObj);
        }

        private void PropertyGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
        }


        private void DGEditStyleTmpl_LostFocus(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("lost");
            try
            {
                PropertyGridItem item = PropertyGrid.SelectedItem as PropertyGridItem;
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
                else if (sender is ComboBox)
                {
                    var b = sender as ComboBox;
                    item.SetValue(b.SelectedValue);
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

        private void cmdViewSubObject_Click(object sender, RoutedEventArgs e)
        {
            PropertyGridItem propItem = PropertyGrid.SelectedItem as PropertyGridItem;
            if (propItem == null) return;
            IPropertyInspectable item = propItem.ValueObj as IPropertyInspectable;
            if (item == null) return;
            PropertyInspect pi = new PropertyInspect(item);
            pi.Show();
        }


    }
}
