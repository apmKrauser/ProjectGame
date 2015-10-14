using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleGraphicsLib
{
    public class GFXComposition : IGFXObjComposition 
    {
        public event Action<DrawingVisual> RegisterDrawingVisual;

        public event Action<DrawingVisual> UnregisterDrawingVisual;

        ObservableCollection<IGFXObject> Children = new ObservableCollection<IGFXObject>();

        public GFXComposition()
        {   }

        public GFXComposition(string name)
        {
            this.Name = name;
        }

        private GFXContainer _parentContainer = null;
        public GFXContainer ParentContainer
        {
            get
            {
                return _parentContainer;
            }
            set
            {
                _parentContainer = value;
                // ensure event hierarchy and consitency
                foreach (var item in Children)
                {
                    if (_parentContainer != null)
                    {
                        item.RegisterDrawingVisual   += this.RegisterDrawingVisual;
                        item.UnregisterDrawingVisual += this.UnregisterDrawingVisual;
                    }
                    else
                    {
                        item.RegisterDrawingVisual   -= this.RegisterDrawingVisual;
                        item.UnregisterDrawingVisual -= this.UnregisterDrawingVisual;
                    }
                    item.ParentContainer = _parentContainer;
                }
            }
        }

        public string Name { get; set; }

        public void Frame_Update(object sender, FrameUpdateEventArgs e)
        {
            foreach (var item in Children)
            {
                item.Frame_Update(sender, e);
            }
        }

        public void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            foreach (var item in Children)
            {
                item.Animation_Update(sender, e);
            }            
        }

        public void Dispose()
        {
            foreach (var item in Children)
            {
                item.Dispose();
            }
            Children.Clear();
        }

        ~GFXComposition()
        {
            this.Dispose();
        }

        public void AddObject(IGFXObject obj)
        {
            Children.Add(obj);
        }

        public void RemoveObject(IGFXObject obj)
        {
            Children.Remove(obj);            
        }

        public ObservableCollection<IGFXObject> GetChildren()
        {
            return Children;
        }


        public bool Animated
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool Highlight
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool ContainsVisual(Visual v)
        {
            throw new NotImplementedException();
        }
    }
}
