using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPropertyInspector
{
    public interface IPropertyInspectable
    {
        string Name { get; set; }

        string TypeName { get; set; }
    }
}
