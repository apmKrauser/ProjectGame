using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public class Helper
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
    }
}
