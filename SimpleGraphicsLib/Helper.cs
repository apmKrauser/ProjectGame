using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleGraphicsLib
{
    public static class Helper
    {

        public const string DataDir = "data";

        public static string AssemblyUriStr
        {
            get
            {
                //string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                //UriBuilder uri = new UriBuilder(codeBase);
                //string path = Uri.UnescapeDataString(uri.Path);
                //return Path.GetDirectoryName(path);
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
                dlgOpen.Filter = "json|*.json|*.*|*.*";
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


    }
}
