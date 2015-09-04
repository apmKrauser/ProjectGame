using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class SpriteObjectElastic : SpriteObject, IElasticBody
    {
        private  double _springC;
        private  double _dampingC;

        [DataMember]
        public double SpringC
        {
            get { return _springC; }
            set { _springC = value == 0 ? 0.1 : value; }
        }

        [DataMember]
        public double DampingC
        {
            get { return _dampingC; }
            set { _dampingC = value == 0 ? 0.1 : value; }
        }

        [DataMember]
        public bool IsLiquid
        {
            get;
            set;
        }

        [DataMember]
        public bool IsDeformable
        {
            get;
            set;
        }

        public SpriteObjectElastic() : this("") { }

        public SpriteObjectElastic(string name) : base(name)
        {
            SpringC = DampingC = 0;
            IsDeformable = IsLiquid = false;
        }

    }
}
