using System;
namespace SimpleGraphicsLib
{
    public interface IGFXAnimation : IDisposable
    {
        bool IsActive { get; set; }
        void Start();
        void Stop();
        void SetTimingSource(GFXContainer _GFXContainer);
        void SetTimingSource(TimingSource.Sources _tsrc);
        void Update(object sender, FrameUpdateEventArgs e);
    }
}
