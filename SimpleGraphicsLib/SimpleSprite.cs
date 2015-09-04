using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class SimpleSprite : SpriteObject
    {


        // test wird sogar ausgeführt
        protected override void init()
        {
            DrawingVisual vis = new DrawingVisual();
            Visuals.Add(vis);
            System.Windows.Forms.MessageBox.Show("Bäääääää");
        }
    }
}
