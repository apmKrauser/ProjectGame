using MahApps.Metro.Controls;
using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BobbleCar
{
    /// <summary>
    /// Updates jump power progress bar and other WPF GUI elements 
    /// Jump progress bar: Player game object has to be named 'Player', Jump animatior's name: 'Jump'
    /// </summary>
    public class GUIAnimator : GFXAnimation, IDisposable, IGFXAnimation
    {

        GFXContainer Gfx;
        LevelSet Lvl;
        MetroProgressBar pgJumpRes;

        public GUIAnimator()
        {

        }

        public GUIAnimator(GFXContainer gfx, LevelSet lvl, TimingSource.Sources timingSrc, MetroProgressBar pgJumpRes)
        {
            this.Gfx = gfx;
            this.Lvl = lvl;
            this.pgJumpRes = pgJumpRes;
            IsActive = true;
            SetTimingSource(timingSrc);
        }


        public override void Update_Active(object sender, FrameUpdateEventArgs e)
        {
            // Update damper power
            if (Gfx != null)
            {
                CarObject player = Gfx.FindObject("Player") as CarObject;
                if (player != null)
                {
                    AnimKeyJump aniJump = player.GetAnimation("Jump") as AnimKeyJump;
                    if (aniJump != null)
                        pgJumpRes.Value = aniJump.JumpReservoir / aniJump.JumpReservoirMax * 100;
                }
            } //  damper
            
        }


    }
}
