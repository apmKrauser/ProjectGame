using System;
using System.Collections.Generic;
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
    public class AnimKeyJump : AnimationRigidBody, IAnimKeyInput
    {
        //[MethodImpl]
        [DataMember]
        public Vector JumpDirection { get; set; }

        [DataMember]
        public double StoredJumpPower { get; set; }

        [DataMember]
        public double JumpReservoir { get; set; }

        [DataMember]
        public double JumpReservoirMax { get; set; }

        [DataMember]
        public double JumpFillSpeed { get; set; }

        [DataMember]
        public double JumpPowerRestore { get; set; }

        [DataMember]
        public bool NeedsGround { get; set; }

        [DataMember]
        public Key KeyJump { get; set; }

        bool loadJump = false;


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
            // load reservoir
            JumpReservoir += JumpPowerRestore * e.ElapsedMilliseconds/1000;
            JumpReservoir = Math.Min(JumpReservoir, JumpReservoirMax);

            // load jump
            if (loadJump)
            {
                double dp = Math.Min(JumpFillSpeed * e.ElapsedMilliseconds / 1000, JumpReservoir);
                if (dp < 0) dp = 0;
                JumpReservoir -= dp;
                StoredJumpPower += dp;
                if (JumpReservoir < 0) JumpReservoir = 0;
                StoredJumpPower = Math.Min(JumpReservoirMax, StoredJumpPower);
            }

            // release jump
            if (!loadJump)
            {
                if ((!NeedsGround || Sprite.IsGrounded) && (StoredJumpPower > 0))
                {
                    // Jump
                    Sprite.NormSpeed += StoredJumpPower * JumpDirection;
                }
                else
                {
                    JumpReservoir += StoredJumpPower; // Jump not working
                }
                StoredJumpPower = 0;
            }
            var el = Sprite as SpriteObjectElastic;
            if (el != null)
            {
                double bend = (StoredJumpPower / JumpReservoirMax) * 0.4;
                el.Deformation = new Rect(el.Deformation.X, el.Deformation.Y + bend, el.Deformation.Width, el.Deformation.Height - bend);
            }

        }
       
        
        public AnimKeyJump()
        {
            NeedsGround = true;
            KeyJump = Key.Up;
            JumpReservoir = 200;
            JumpReservoirMax = 200;
            StoredJumpPower = 0;
            JumpPowerRestore = JumpReservoirMax / 5;
            JumpFillSpeed = JumpReservoirMax / 2;
            JumpDirection = new Vector(0, -1);
        }


        ~AnimKeyJump()
        {
            Dispose();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            SpriteObject sobj = Sprite as SpriteObject;
            if (e.Key == KeyJump)
            {
                loadJump = true;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == KeyJump)
            {
                loadJump = false;
            }
        }

    }
}
