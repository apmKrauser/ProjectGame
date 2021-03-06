﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SimpleGraphicsLib
{
    public interface IGFXObject : IDisposable, IGFXObjComposable<IGFXObject>
    {
        event Action<DrawingVisual> RegisterDrawingVisual;

        event Action<DrawingVisual> UnregisterDrawingVisual;

        GFXContainer ParentContainer { get; set; }

        bool ContainsVisual(System.Windows.Media.Visual v);

        bool Highlight { get; set; }

        string Name { get; set; }

        bool Animated { get; set; }

        void Frame_Update(object sender, FrameUpdateEventArgs e);

        void Animation_Update(object sender, FrameUpdateEventArgs e);
    }
}
