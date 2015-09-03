﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace SimpleGraphicsLib
{
    public class AnimationAirDrag : AnimationRigidBody, IAnimatonRigidBody
    {

        public AnimationAirDrag(IRigidBody _sprite) : base(_sprite) {}


        public override void Update(object sender, FrameUpdateEventArgs e)
        {
            if (Sprite.IsMovable)
            {
                // F = k*v 
                // dv = F/m *dt
                Vector A_drag = -Sprite.AirDrag * Sprite.NormSpeed;
                if (Sprite.Weight > 0)
                        A_drag /= Sprite.Weight;  // calculate without weight if there is none
                Vector dpos = (A_drag) * (e.ElapsedMilliseconds / 1000);
                Sprite.NormSpeed += dpos;
            }
        }

    }
}
