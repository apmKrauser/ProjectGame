using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IAnimKeyInput 
    {

        void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e);
        void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e);
    }
}
