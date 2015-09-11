using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleGraphicsLib
{
    public interface IHasSeperateAnimationEvent
    {
        void Animation_Update(object sender, FrameUpdateEventArgs e);
    }
}
