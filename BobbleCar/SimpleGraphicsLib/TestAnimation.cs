using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;


namespace SimpleGraphicsLib
{
    [DataContract]
    public class TestAnimation : AnimationRigidBody, IAnimationRigidBody
    {


        double sinex = 0;
        Point SpritePos;
        Point SpriteSize;
        private double SpriteAngle = 0; // debug



        public TestAnimation(){}

        public TestAnimation(IRigidBody _sprite)
        {
            Sprite = _sprite;
        }

        // tiggered by GFXContainer
        public TestAnimation(IRigidBody _sprite, TimingSource.Sources _tsrc)
        {
            Sprite = _sprite;
            SetTimingSource(_tsrc);
        }

        public TestAnimation(IRigidBody _sprite, GFXContainer _GFXContainer)
        {
            Sprite = _sprite;
            SetTimingSource(_GFXContainer);
        }

        public override void Update(object sender, FrameUpdateEventArgs e)
        {
            if (Sprite == null)
            {
                this.Dispose();
                return;
            }

            //if (sinex > 2*Math.PI) sinex = 0;
            sinex %= 2 * Math.PI;
            sinex += 2.5;
            SpriteAngle += 1.5;
            SpriteAngle %= 360;
            //SpriteBlur += 0.5;
            //SpriteBlur %= 20;
            // wrappanel benutzen für crop?
            Vector dpos = new Vector(Math.Round(Math.Sin(sinex) * 1),
                      Math.Round(Math.Sin(sinex) * 5));
            Vector dsize = new Vector(Math.Round(dpos.X / 2.0), -dpos.Y);

            Point newPos = Point.Add(SpritePos, dpos);
            Size newSize = (Size)Point.Add((Point)SpriteSize, dsize);

            Sprite.Angle = SpriteAngle;
            Sprite.Position = new Vector(SpriteAngle, 100);
        }




    }
}
