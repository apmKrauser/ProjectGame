﻿using SimpleGraphicsLib;
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
using Xceed.Wpf.Toolkit;

namespace BobbleCar
{
    /// <summary>
    /// Property window for game objects
    /// </summary>
    public partial class PropertyInspect : MetroWindow
    {
        IPropertyInspectable GObj;

//        public ObservableCollection<PropertyGridSet> PList = new ObservableCollection<PropertyGridSet>();
        public List<PropertyGridItem> PList = new List<PropertyGridItem>();


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

        /// <summary>
        /// Show animators of game object in animator browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Read all properties of a game object
        /// </summary>
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

        /// <summary>
        /// update a certain property
        /// </summary>
        /// <param name="propItem"></param>
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
            // todo: sometimes buggy behavior on checkboxes/textboxes; selected item wrong
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
            PropertyInspect pi = new PropertyInspect(item);
            pi.Show();
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
                        UpdateProperty(item);
                    }
                }
                
            }
            catch (Exception ex)
            { } 
        }


    }
}
