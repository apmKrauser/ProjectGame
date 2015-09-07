using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleGraphicsLib
{
    public interface IRigidBody
    {
        Vector NormSpeed
        {
            get;
            set;
        }

        Vector Position
        {
            get;
            set;
        }

        Vector SizeV
        {
            get;
            set;
        }

        Vector CenterOfMass // relative 0 = Pos; 1 = Pos+Size also rotational center
        {
            get;
            set;
        }

        double Weight
        {
            get;
            set;
        }

        double AirDrag
        {
            get;
            set;
        }

        Vector PixelSpeed
        {
            get;
            //set;
        }

        double Angle 
        { 
            get; 
            set; 
        }

        Rect Shape
        {
            get;
        }
        
        bool IsMovable { get; set; }
        bool IsObstacle { get; set; }
        bool CanCollide { get; set; }
        bool IsGrounded { get; set; }

    }
}
