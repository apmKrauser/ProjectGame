using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class AnimationWobble : AnimationRigidBody
    {

        double sinex = 0;

        [DataMember]
        public double WobbleSpeed { get; set; }



        [DataMember]
        public double WobbleAmplitude { get; set; }

        public AnimationWobble()
        {     }

        public AnimationWobble(double WobbleSpeed, double WobbleAmplitude)
        {
            this.WobbleSpeed = WobbleSpeed;
            this.WobbleAmplitude = WobbleAmplitude;
        }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context)
        {
            sinex = 0;
        }

        public override void Update_Active(object sender, FrameUpdateEventArgs e)
        {
            //sinex += WobbleSpeed * e.ElapsedMilliseconds;
            sinex += WobbleSpeed * 10;

            IElasticBody ESprite = Sprite as IElasticBody;
            if (ESprite != null)
            {
                Vector dpos = new Vector(Math.Sin(sinex)*2, Math.Sin(sinex)*5);
                dpos *= WobbleAmplitude;
                Vector dsize = new Vector(dpos.X / 2.0, -dpos.Y);

                ESprite.Deformation = new Rect(         ESprite.Deformation.Location + dpos,
                                                (Vector)ESprite.Deformation.Size + dsize);
            }
        }
    }
}
