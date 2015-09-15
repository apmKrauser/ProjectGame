using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{

    [DataContract]
    public class AnimKeyDrive : AnimationRigidBody, IAnimKeyInput
    {
        [DataMember]
        //[MethodImpl]
        public Vector Acceleration { get; set; }

        [DataMember]
        public double VMax { get; set; }

        [DataMember]
        public double AccBreak { get; set; }

        [DataMember]
        public bool NeedsGround { get; set; }

        [DataMember]
        public Key KeyForward { get; set; }

        [DataMember]
        public Key KeyBackward { get; set; }

        [DataMember]
        public Key KeyBreak { get; set; }

        private bool breaking = false;



        public override BitmapImage Symbol
        {
            get
            {
                return Properties.Resources.input.ToMediaBitmap();
            }
        }

        private Vector currentAccel;


        public override void Update_Active(object sender, FrameUpdateEventArgs e)
        {
            if (currentAccel == null) currentAccel = new Vector(0, 0);

            if (!NeedsGround || Sprite.IsGrounded)
            {
                Vector dv = currentAccel * (e.ElapsedMilliseconds / 1000);
                // speen in currentAccel direction
                if ((dv.Length > 0) && (Sprite.NormSpeed.Length > 0))
                {
                    double s = Vector.Multiply(Sprite.NormSpeed, currentAccel) / currentAccel.Length;
                    if (s > VMax) dv = new Vector(0, 0);
                }
                Sprite.NormSpeed += dv;
                // breaking
                double dbreak = AccBreak * (e.ElapsedMilliseconds / 1000);
                if (breaking)
                {
                    if (Sprite.NormSpeed.Length <= dbreak)
                        Sprite.NormSpeed = new Vector(0, 0);
                    else
                    {
                        dv = Sprite.NormSpeed;
                        dv.Normalize();
                        dv *= dbreak;
                        Sprite.NormSpeed -= dv;
                    }
                } 
            }
        }
       
        
        public AnimKeyDrive()
        {
            NeedsGround = true;
            KeyForward = Key.Right;
            KeyBackward = Key.Left;
            KeyBreak = Key.Down;
            Acceleration = new Vector(20,0);
            AccBreak = 30;
            VMax = 60;
        }


        ~AnimKeyDrive()
        {
            Dispose();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            SpriteObject sobj = Sprite as SpriteObject;
            if (e.Key == KeyForward)
            {
                currentAccel = Acceleration;
                if (sobj != null) sobj.FlipHorizontal = false;
            }
            if (e.Key == KeyBackward)
            {
                currentAccel = -Acceleration;
                if (sobj != null) sobj.FlipHorizontal = true;
            }
            if (e.Key == KeyBreak)
            {
                breaking = true;
                currentAccel = new Vector(0, 0);
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == KeyBreak)
             || (e.Key == KeyBackward)
             || (e.Key == KeyForward))
            {
                currentAccel = new Vector(0, 0);
                breaking = false;
            }
        }


    }
}
