using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace SimpleGraphicsLib
{
    [DataContract]
    public class AnimationBindProperty : AnimationRigidBody, IAnimationRigidBody
    {


        public enum BindingProperty
        {
            PosX,
            PosY
        }

        [DataMember]
        public BindingProperty BoundProp { get; set; } 

        [DataMember]
        public string BoundSpriteName { get; set; }

        [DataMember]
        public double BindScaling { get; set; }

        [DataMember]
        public bool TwoWayBinding { get; set; }

        private double? oldValue = null;


        public AnimationBindProperty()
        {
            BoundProp = BindingProperty.PosY;
            BindScaling = 1;
            BoundSpriteName = "";
        }

        public AnimationBindProperty(IRigidBody _sprite) : base(_sprite) {}


        public override void Update_Active(object sender, FrameUpdateEventArgs e)
        {
            IGameObject ThisObj  = Sprite as IGameObject;
            IGameObject OtherObj = null;
            if (ThisObj != null)
                OtherObj = ThisObj.ParentContainer.FindObject(BoundSpriteName) as IGameObject;
            if (ThisObj != null && OtherObj != null)
            {
                // begin binding / first start
                if (!oldValue.HasValue)
                {
                    switch (BoundProp)
                    {
                        case BindingProperty.PosX:
                            oldValue = OtherObj.Position.X;
                            break;
                        case BindingProperty.PosY:
                            oldValue = OtherObj.Position.Y;
                            break;
                        default:
                            break;
                    }
                }

                switch (BoundProp)
                {
                    case BindingProperty.PosX:
                        ThisObj.Position += new Vector(OtherObj.Position.X - oldValue.Value, 0) * BindScaling;
                        oldValue = OtherObj.Position.X;
                        break;
                    case BindingProperty.PosY:
                        ThisObj.Position += new Vector(0, OtherObj.Position.Y - oldValue.Value) * BindScaling;
                        oldValue = OtherObj.Position.Y;
                        break;
                }
            } else {
                oldValue = null;
            }
        }

    }
}

