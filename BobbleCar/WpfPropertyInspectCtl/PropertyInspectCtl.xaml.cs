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
using Xceed.Wpf.Toolkit;

namespace WPFPropertyInspector
{
    /// <summary>
    /// Interaktionslogik für PropertyInspectCtl.xaml
    /// </summary>
    public partial class PropertyInspectCtl : UserControl
    {

        // WPF Event Schritt 1: erzeugen
        public static readonly RoutedEvent InspectSubObjectEvent =
            EventManager.RegisterRoutedEvent("InspectSubObject",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(PropertyInspectCtl));

        // WPF Event Schritt 1: Wrapper für Eventhandler

        //[System.ComponentModel.Description("Invoke")]
        //[System.ComponentModel.Category("Invoke Category")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public event RoutedEventHandler InspectSubObject
        {
            add
            {
                this.AddHandler(InspectSubObjectEvent, value);  // +=
            }
            remove
            {
                this.RemoveHandler(InspectSubObjectEvent, value);  // -=
            }
        }



        // Using a DependencyProperty as the backing store for ObjectToInspect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ObjectToInspectProperty =
            DependencyProperty.Register("ObjectToInspect", typeof(IPropertyInspectable), typeof(PropertyInspectCtl), new PropertyMetadata(null, (o,e) => ((PropertyInspectCtl)o).OnObjectToInspectChanged() ));

        public IPropertyInspectable ObjectToInspect
        {
            get { return (IPropertyInspectable)GetValue(ObjectToInspectProperty); }
            set 
            { 
                SetValue(ObjectToInspectProperty, value);
                OnObjectToInspectChanged();
            }
        }

        //public IPropertyInspectable ObjectToInspect
        //{
        //    get
        //    { 
        //        return GObj;
        //    }

        //    set
        //    {
        //        GObj = value;
        //        PropertyGrid.ItemsSource = PList;
        //        ReadProperties();
        //        PropertyGrid.Items.Refresh();
        //    }
        //}

        private void OnObjectToInspectChanged()
        {
            ReadProperties();
            PropertyGrid.Items.Refresh();
        }


        public PropertyInspectCtl()
        {
            InitializeComponent();
            PropertyGrid.ItemsSource = PList;
        }

        // IPropertyInspectable GObj;

//        public ObservableCollection<PropertyGridSet> PList = new ObservableCollection<PropertyGridSet>();
        public List<PropertyGridItem> PList = new List<PropertyGridItem>();
        //public BindingList<PropertyGridSet> PList = new BindingList<PropertyGridSet>();
        



        private void ReadProperties()
        {
            PList.Clear();
            //Assembly.GetExecutingAssembly();
            var typ = ObjectToInspect.GetType();
            foreach (var item in typ.GetProperties())
            {
                object v = item.GetValue(ObjectToInspect);
                var t = item.PropertyType;
                if (((v is String) ||
                   (v is Double) ||
                   (v is int) ||
                   (v is bool) ||
                   (v is Key) ||
                   (v is Enum) ||
                   (v is Vector) ||
                   (v is Rect) ||
                   (v is Color) 
                   ) ||
                   (v is IPropertyInspectable)
                    && item.CanWrite)
                {
                    PList.Add(new PropertyGridItem { Name = item.Name, Type = t.ToString(), ValueObj = v });
                }
            }

        }

        private void UpdateProperty(PropertyGridItem propItem)
        {
            var typ = ObjectToInspect.GetType();
            var prop = typ.GetProperty(propItem.Name);
            prop.SetValue(ObjectToInspect, propItem.ValueObj);
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
            // Todo: Event für Unterobject implementieren!!
            RaiseEvent(new PropInspRoutedEventArgs(InspectSubObjectEvent, item));
            //RaiseEvent(new RoutedEventArgs(InspectSubObjectEvent));
            //PropertyInspect pi = new PropertyInspect(item);  
            //pi.Show();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PropertyGridItem item = PropertyGrid.SelectedItem as PropertyGridItem;
                if (item != null)
                {
                    if (sender is ComboBox)
                    {
                        var b = sender as ComboBox;
                        item.SetValue(b.SelectedValue);
                       // Debug.WriteLine("=> Selected = " + b.SelectedValue + ":" + b.SelectedValue.GetType().Name);
                      //  Debug.WriteLine("=> Sett = " + item.ValueAsKey + ":" + item.ValueObj.GetType().Name);
                        UpdateProperty(item);
                        //PropertyGrid.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {            }           
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                PropertyGridItem item = PropertyGrid.SelectedItem as PropertyGridItem;
                if (item != null)
                {
                    if (sender is ColorPicker)
                    {
                        var b = sender as ColorPicker;
                        item.SetValue(b.SelectedColor);
                        // Debug.WriteLine("=> Selected = " + b.SelectedValue + ":" + b.SelectedValue.GetType().Name);
                        //  Debug.WriteLine("=> Sett = " + item.ValueAsKey + ":" + item.ValueObj.GetType().Name);
                        UpdateProperty(item);
                        //PropertyGrid.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            { } 
        }

    }
}
