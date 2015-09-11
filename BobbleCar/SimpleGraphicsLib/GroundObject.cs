using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    [DataContract]
    class GroundObject : SpriteObjectElastic
    {
        public GroundObject() : this("") { }

        public GroundObject(string name)
            : base(name)
        {
            SpringC = DampingC = 0;
            IsDeformable = IsLiquid = false;
            IsMovable = false;
            CanCollide = true;
            IsObstacle = false;
        }
    }
}
