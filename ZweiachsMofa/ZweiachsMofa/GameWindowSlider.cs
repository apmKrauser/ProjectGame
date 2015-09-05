using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZweiachsMofa
{
    public class GameWindowSlider : Slider, IRigidBody
    {

        private Vector _position;

        public Vector NormSpeed
        {
            get;
            set;
        }

        public Vector Position
        {
            get
            {
                return new Vector(this.Value, 0);
            }
            set
            { _position = value; }
        }

        public Vector SizeV
        {
            get;
            set;
        }

        public Vector CenterOfMass // relative 0 = Pos; 1 = Pos+Size also rotational center
        {
            get;
            set;
        }

        public double Weight
        {
            get;
            set;
        }

        public double AirDrag
        {
            get;
            set;
        }

        public Vector PixelSpeed
        {
            get { return new Vector(0, 0); }
            //set;
        }

        public double Angle
        {
            get;
            set;
        }

        public Rect Shape
        {
            get { return new Rect(0, 0, 0, 0); }
        }

        public bool IsMovable { get; set; }
        public bool CanCollide { get; set; }

        public bool IsObstacle { get; set; }
    }
}
