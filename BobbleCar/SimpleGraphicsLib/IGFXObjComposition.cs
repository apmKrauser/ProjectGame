using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IGFXObjComposition<T>
    {
        void AddObject(T obj);

        void RemoveObject(T obj);

        ObservableCollection<T> GetChildren();
    }
}
