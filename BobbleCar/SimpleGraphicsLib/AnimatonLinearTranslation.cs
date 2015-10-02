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
    public class AnimationLinearTranslation : AnimationRigidBody
    {
        //public IRigidBody Sprite { get; set; }

        //public event Action<IAnimatonRigidBody> OnDispose;

        public override System.Windows.Media.Imaging.BitmapImage Symbol
        {
            get
            {
                return Properties.Resources.locked.ToMediaBitmap();
            }
        }
        public AnimationLinearTranslation(IRigidBody _sprite) : base(_sprite) {}

        public AnimationLinearTranslation()  {}


        public override void Update_Active(object sender, FrameUpdateEventArgs e)
        {
            if (Sprite.IsMovable)
            {
                Vector nSpeed = new Vector();
                IElasticBody ESprite = Sprite as IElasticBody;
                if (ESprite != null)
                    ESprite.Deformation = new Rect(0, 0, 1, 1);
                // check collisions first
                if (Sprite.IsObstacle)
                    (Sprite as IGFXObject).ParentContainer.Collider.Check(Sprite as IGameObject, e);
                // speed threshold in order to avoid oscillations
                nSpeed.X = (Math.Abs(Sprite.NormSpeed.X) < 0.1) ? 0 : Sprite.NormSpeed.X;
                nSpeed.Y = (Math.Abs(Sprite.NormSpeed.Y) < 0.1) ? 0 : Sprite.NormSpeed.Y;
                Vector dpos = GFXAnimation.NormToPixelSpeed(nSpeed) * (e.ElapsedMilliseconds / 1000);
                Sprite.Position += dpos;
                // Reset deformation
            }
        }


    }
}
