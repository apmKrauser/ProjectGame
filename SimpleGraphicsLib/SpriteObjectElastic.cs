using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public class SpriteObjectElastic : SpriteObject, IElasticBody
    {
        private  double _springC;
        private  double _dampingC;


        public double SpringC
        {
            get { return _springC; }
            set { _springC = value == 0 ? 0.1 : value; }
        }

        public double DampingC
        {
            get { return _dampingC; }
            set { _dampingC = value == 0 ? 0.1 : value; }
        }

        public bool IsLiquid
        {
            get;
            set;
        }

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
