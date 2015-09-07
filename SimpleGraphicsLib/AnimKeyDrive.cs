using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public Vector Acceleration { get; set; }

        [DataMember]
        public double VMax { get; set; }

        [DataMember]
        public Key KeyForward { get; set; }

        [DataMember]
        public Key KeyBackward { get; set; }



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

            Vector dv = currentAccel * (e.ElapsedMilliseconds / 1000);
            if ((Math.Abs((Vector.Multiply(Sprite.NormSpeed, dv))) < Math.Abs(VMax * Sprite.NormSpeed.Length)) || (Sprite.NormSpeed.Length == 0)) Sprite.NormSpeed += dv;
        }
       
        
        public AnimKeyDrive()
        {
            KeyForward = Key.Right;
            KeyBackward = Key.Left;
        }


        ~AnimKeyDrive()
        {
            Dispose();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == KeyForward)
            {
                currentAccel = Acceleration;
            }
            if (e.Key == KeyBackward)
            {
                currentAccel = -Acceleration;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            currentAccel = new Vector(0,0);
        }


    }
}
