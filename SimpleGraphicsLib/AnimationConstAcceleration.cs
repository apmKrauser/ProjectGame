using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class AnimationConstAcceleration : AnimationRigidBody
    {
        [DataMember]
        public Vector Acceleration { get; set; }

        public AnimationConstAcceleration()
        {       }

        public AnimationConstAcceleration(Vector Acceleration) : base()  // kann man weglassen?
        {
            // TODO: Complete member initialization
            this.Acceleration = Acceleration;
        }

        public override void Update(object sender, FrameUpdateEventArgs e)
        {
            Vector dpos = Acceleration * (e.ElapsedMilliseconds / 1000);
            Sprite.NormSpeed += dpos;
        }

    }
}
