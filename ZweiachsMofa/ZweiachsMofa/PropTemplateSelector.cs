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

namespace ZweiachsMofa
{

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
                ValueObj = Double.Parse(value);
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
        
        public void SetValue(object value)
        {   // who am i
            if (ValueObj is string)
                ValueAsString = (string)value;
            else if (ValueObj is double)
                ValueAsDouble = (string)value;
            else if (ValueObj is bool)
                ValueAsBool = (bool)value;
            else if (ValueObj is Vector)
                ValueAsVector = (string)value;
        }
        
    }

    public class PropTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ObjectStaticTextTemplate { get; set; }
        public DataTemplate ObjectTextTemplate { get; set; }
        public DataTemplate ObjectDoubleTemplate { get; set; }
        public DataTemplate ObjectBooleanTemplate { get; set; }
        public DataTemplate ObjectVectorTemplate { get; set; }


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
            else if (value is Vector)
            {
                return ObjectVectorTemplate;
            }
            else
                return base.SelectTemplate(item, container);
        }
    }
    public interface ITest
    { }
    public class Test1 : ITest
    {
        public String _a = "ooo";
        public bool _b = false;


        public String a
        {
            get { return _a; }
            set { _a = value; }
        }

        public bool b
        {
            get { return _b; }
            set { _b = value; }
        }
    }

    public class Test2 : ITest
    {
        private String _a = "Trööt";

        public String a
        {
            get { return _a; }
            set { _a = value; }
        }

        private int _b = 5;

        public int b
        {
            get { return _b; }
            set { _b = value; }
        }


    }
}
