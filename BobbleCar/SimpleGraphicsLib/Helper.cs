﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SimpleGraphicsLib
{
    public static class Helper
    {
        public class ObjectWrapper
        {
            public IPropertyInspectable Control { get; private set; }

            public ObjectWrapper(Object control)
            {
                this.Control = (IPropertyInspectable)control; // as IPropertyInspectable;
            }
        }

        public const string DataDir = "data";

        public static string AssemblyUriStr
        {
            get
            {
                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                path = path.Replace(@"\bin\Debug", "");
                return path;
            }
        }


        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, Assembly assembly)
        {
            var types = from t in assembly.GetTypes()
                        where t.IsSubclassOf(baseType)
                        select t;

            return types;
        }

        public static string AssemblyLocalPath
        {
            get
            {
                return (new Uri(AssemblyUriStr)).LocalPath;
            }
        }

        public static string DataLocalPath
        {
            get
            {
                return AssemblyLocalPath + "\\" + DataDir;
            }
        }

        public static String OpenFile()
        {
            String fullpath = null;
            String relpath = "";
            try
            {
                OpenFileDialog dlgOpen = new OpenFileDialog();
                //dlgOpen.Filter = "json|*.json|*.*|*.*";
                dlgOpen.Filter = "XML|*.xml|*.*|*.*";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    fullpath = dlgOpen.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Opening File:\n"  + ex.Message);
            }
            return fullpath;
        }

        public static String SaveFile()
        {
            String fullpath = null;
            String relpath = "";
            try
            {
                SaveFileDialog dlgOpen = new SaveFileDialog();
                //dlgOpen.Filter = "json|*.json|*.*|*.*";
                dlgOpen.Filter = "XML|*.xml|*.*|*.*";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    fullpath = dlgOpen.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Opening File:\n" + ex.Message);
            }
            return fullpath;
        }

    }
}
