using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleGraphicsLib
{
    public interface IElasticBody : IRigidBody
    {

        double SpringC { get; set; }

        double DampingC { get; set; }

        bool IsLiquid { get; set; }

        bool IsDeformable { get; set; }

        Rect Deformation { get; set; }

    }
}
