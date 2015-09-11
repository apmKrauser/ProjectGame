using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IPropertyInspectable
    {
        string Name { get; set; }

        string TypeName { get; set; }
    }
}
