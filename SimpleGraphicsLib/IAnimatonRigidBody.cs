using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public interface IAnimationRigidBody  : IGFXAnimation
    {

        IRigidBody Sprite { get; set; }
        string Name { get; set; }
        event Action<IAnimationRigidBody> OnDispose;

    }
}
