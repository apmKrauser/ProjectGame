using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGraphicsLib
{
    public class SmoothGameScroller : GameScroller
    {

        public double IFkt = 1.5;
        protected double PrefXPos;  // Preferred X Pos 
        
        public SmoothGameScroller(IRigidBody centeredObject, GFXContainer gfxContainer, FrameworkElement viewWindow)
            : base(centeredObject, gfxContainer, viewWindow)
        {

        }

        public override void Update(object sender, FrameUpdateEventArgs e)
        {
            // Left border
            if (CenteredObject.Position.X < (ViewWindow.Width / 2))
            {
                PrefXPos = 0;
            }
            // Middle
            else if (CenteredObject.Position.X > (GContainer.Width - (ViewWindow.Width / 2)))
            {
                PrefXPos = (ViewWindow.Width - GContainer.Width);
            }
            // Right border
            else
            {
                PrefXPos = (ViewWindow.Width / 2) - CenteredObject.Position.X;
            }

            double dx = PrefXPos - GContainer.DrawingOffset.X;
            double XPos = dx * IFkt * (e.ElapsedMilliseconds/1000);
            XPos += GContainer.DrawingOffset.X;
            GContainer.DrawingOffset = new Vector(XPos, 0);

        }

    }
}
