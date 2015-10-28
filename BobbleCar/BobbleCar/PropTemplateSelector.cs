//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using SimpleGraphicsLib;

namespace BobbleCar
{

    /// <summary>
    /// Template selector for PropertyInspect window
    /// </summary>
    public class PropertyGridItem
    {
        public String Type { get; set; }
        public String Name { get; set; }
        public Object ValueObj { get; set; }

        public bool ValueAsBool
        {
            get 
            {
                return (bool)ValueObj;
            }

            set 
            {
                ValueObj = value;
            }
        }

        public string ValueAsString
        {
            get
            {
                return (string)ValueObj;
            }

            set
            {
                ValueObj = value;
            }
        }

        public string ValueAsDouble
        {
            get
            {
                return ((double)ValueObj).ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                ValueObj = Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        public string ValueAsInteger
        {
            get
            {
                return ((int)ValueObj).ToString();
            }

            set
            {
                ValueObj = int.Parse(value);
            }
        }

        public Enum ValueAsKey
        {
            get
            {
                return (Key)ValueObj;
            }

            set
            {
                ValueObj = (value);
            }
        }

        public Enum ValueAsEnum
        {
            get
            {
                return (Enum)ValueObj;
            }

            set
            {
                ValueObj = (value);
            }
        }

        public Array ValueGetEnums
        {
            get
            {
                return Enum.GetValues(ValueObj.GetType());
            }
        }

        public String ValueAsVector
        {
            get
            {
                return ((Vector)ValueObj).ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                ValueObj = Vector.Parse(value);
            }
        }

        public Color ValueAsColor
        {
            get
            {
                return (Color)(ValueObj);
            }

            set
            {
                ValueObj = value;
            }
        }

        public String ValueAsRect
        {
            get
            {
                return ((Rect)ValueObj).ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                ValueObj = Rect.Parse(value);
            }
        }
        
        public void SetValue(object value)
        {   // who am i
            if (ValueObj is string)
                ValueAsString = (string)value;
            else if (ValueObj is double)
                ValueAsDouble = ((string)value);
            else if (ValueObj is int)
                ValueAsInteger = (string)value;
            else if (ValueObj is bool)
                ValueAsBool = (bool)value;
            else if (ValueObj is Key)
                ValueAsKey = (Key)value;
            else if (ValueObj is Enum)
                ValueAsEnum = (Enum)value;
            else if (ValueObj is Vector)
                ValueAsVector = (string)value;
            else if (ValueObj is Rect)
                ValueAsRect = (string)value;
            else if (ValueObj is Color)
                ValueAsColor = (Color)value;
        }
        
    }

    public class PropTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ObjectStaticTextTemplate { get; set; }
        public DataTemplate ObjectTextTemplate { get; set; }
        public DataTemplate ObjectDoubleTemplate { get; set; }
        public DataTemplate ObjectIntTemplate { get; set; }
        public DataTemplate ObjectBooleanTemplate { get; set; }
        public DataTemplate ObjectKeyEnumTemplate { get; set; }
        public DataTemplate ObjectEnumTemplate { get; set; }
        public DataTemplate ObjectVectorTemplate { get; set; }
        public DataTemplate SubObjectTemplate { get; set; }
        public DataTemplate ObjectColorTemplate { get; set; }
        public DataTemplate ObjectRectTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PropertyGridItem propgs = (item as PropertyGridItem);
            object value = null;
            if (propgs != null)
                value = propgs.ValueObj;

            if (value != null)
            if (value is String)
            {
                if (propgs.Name.Equals("Name") || propgs.Name.Equals("TypeName"))
                    return ObjectStaticTextTemplate;
                else
                    return ObjectTextTemplate;
            }
            if (value is Boolean)
            {
                return ObjectBooleanTemplate;
            }
            else if (value is double)
            {
                return ObjectDoubleTemplate;
            }
            else if (value is int)
            {
                return ObjectIntTemplate;
            }
            else if (value is Key)
            {
                return ObjectKeyEnumTemplate;
            }
            else if (value is Enum)
            {
                return ObjectEnumTemplate;
            }
            else if (value is Vector)
            {
                return ObjectVectorTemplate;
            }
            else if (value is Color)
            {
                return ObjectColorTemplate;
            }
            else if (value is Rect)
            {
                return ObjectRectTemplate;
            }
            else if (value is IPropertyInspectable)
            {
                return SubObjectTemplate;
            }
            else
                return base.SelectTemplate(item, container);
        }
    }
}
