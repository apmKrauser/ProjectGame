using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IAnimatonRigidBody  : IGFXAnimation
    {

        IRigidBody Sprite { get; set; }
        string Name { get; set; }
        event Action<IAnimatonRigidBody> OnDispose;

    }
}
