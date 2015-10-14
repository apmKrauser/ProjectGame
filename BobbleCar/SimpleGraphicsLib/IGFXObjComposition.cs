using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IGFXObjComposable<T>
    {

        ObservableCollection<T> GetChildren();
    }

    public interface IGFXObjComposition : IGFXObject
    {
        void AddObject(IGFXObject obj); 

        void RemoveObject(IGFXObject obj);
    }
}
