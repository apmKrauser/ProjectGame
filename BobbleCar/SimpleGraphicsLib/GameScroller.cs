using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGraphicsLib
{
    public class GameScroller : IDisposable
    {

        protected IRigidBody CenteredObject;
        protected GFXContainer GContainer;
        protected FrameworkElement ViewWindow;

        public GameScroller(IRigidBody centeredObject, GFXContainer gfxContainer, FrameworkElement viewWindow)
        {
            this.CenteredObject = centeredObject;
            this.GContainer = gfxContainer;
            this.ViewWindow = viewWindow;
            GContainer.UpdateFrame += Update;
        }

        

        public virtual void  Update(object sender, FrameUpdateEventArgs e)
        {
            // Left border
            if (CenteredObject.Position.X < (ViewWindow.Width /2))
            {
                GContainer.DrawingOffset = new Vector(0, 0);
            }
            // Middle
            else if (CenteredObject.Position.X > (GContainer.Width - (ViewWindow.Width / 2)))
            {
                GContainer.DrawingOffset = new Vector((ViewWindow.Width - GContainer.Width), 0);
            }
            // Right border
            else
            {
                GContainer.DrawingOffset = new Vector((ViewWindow.Width / 2) - CenteredObject.Position.X, 0);
            }
        }

        ~GameScroller()
        {
            Dispose();
        }

        public void Dispose()
        {
            GContainer.UpdateFrame -= Update;            
        }
    }
}
