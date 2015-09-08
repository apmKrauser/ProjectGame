using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGraphicsLib
{
    public class SimpleCollider
    {

        List<IRigidBody> Obstacles = new List<IRigidBody>();

        public SimpleCollider()
        {

        }

        public void AddObject(IRigidBody obj)
        {
            Obstacles.Add(obj);
        }

        public void RemoveObject(IRigidBody obj)
        {
            Obstacles.Remove(obj);
        }

        //[MethodImpl(MethodImplOptions.Synchronized)] // single threaded
        public void Check(IRigidBody me, FrameUpdateEventArgs e)
        {
            Rect rme = me.Shape;
            bool hitsGround = false;
            foreach (var other in Obstacles)
            {
                if (other != me)
                {
                    Rect rother = other.Shape;
                    if (rme.IntersectsWith(rother))
                    {
                        Rect overlap = Rect.Intersect(rme,rother);
                        if (other is GroundObject)
                            hitsGround = true;
                        //Vector overlap = new Vector(rme.Width, rme.Height);
                        if (other.IsObstacle)
                        {
                            ProcessCollision(me, other, overlap, rme, rother, e.ElapsedMilliseconds / 1000);
                            //break;
                        }
                    }
                }
            }
            me.IsGrounded = hitsGround;
        }

        public void ProcessCollision(IRigidBody me, IRigidBody other, Rect overlap, Rect rme, Rect rother, double dt)
        {
            //double overlap = voverlap.Length;
            /* if beide IBallistic
            Vector cogme = new Vector(me.Size.X * me.CenterOfMass.X, me.Size.Y * me.CenterOfMass.Y);
            Vector cogother = new Vector(other.Size.X * other.CenterOfMass.X, other.Size.Y * other.CenterOfMass.Y);
            cogme += me.Position;
            cogother += other.Position;
            */
            // me gegen fixed_obstacle 
            double eps = 1;
            Vector voverlap = new Vector(overlap.Width, overlap.Height);
            Vector dx = new Vector(0, 0);
            if ((int)overlap.Width == (int)rme.Width)
            {
                dx = voverlap;
                if (rme.Top < rother.Top)
                    dx *= -1;
                dx.X = 0;
            }
            else if ((int)overlap.Height == (int)rme.Height)
            {
                dx = voverlap;
                if (rme.Left < rother.Left)
                    dx *= -1;
                dx.Y = 0;
            } 
            else
            {
                Vector v1 = rme.TopLeft - rother.TopLeft;
                Vector v2 = rme.TopRight - rother.BottomLeft;
                Vector v3 = rme.BottomRight - rother.TopLeft;
                Vector v4 = rme.BottomLeft - rother.TopRight;

                if (Math.Abs(voverlap.Length - v3.Length) < eps)
                {
                    dx = -v3;
                }
                else if (Math.Abs(voverlap.Length - v4.Length) < eps)
                { dx = -v4; }
                else if (Math.Abs(voverlap.Length - v2.Length) < eps)
                { dx = -v2; }
                else if (Math.Abs(voverlap.Length - v1.Length) < eps)
                { dx = -v1; }
            }

            #region Animation
            // animation
            if (other.IsMovable)
            {
            }
            else // not Movable
            {
                if (me is IElasticBody)
                {
                    IElasticBody mel = (me as IElasticBody);
                    //if mel.IsMovable
                    double SpringC = mel.SpringC;
                    double Damping = mel.DampingC;
                    bool liquid = false;
                    if (other is IElasticBody)
                    {
                        IElasticBody otherel = (other as IElasticBody);
                        SpringC = 1 / ((1 / SpringC) + (1 / otherel.SpringC));
                        Damping = (Damping + otherel.DampingC) / 2;
                        liquid = otherel.IsLiquid;

                        if (dx.Length > 0)
                        {
                            //Vector deform = new Vector(dx.X, dx.Y);
                            mel.NormSpeed += dx * dt * SpringC; // masse ignorieren
                            double bend = dx.Length / ((Vector)me.Shape.Size).Length;
                            if (bend > 0.5) bend = 0.5; // maximales eindellen
                            dx.Normalize();
                            if (liquid)
                            {
                                // Water animator
                                Vector vdamp = -Math.Abs(Vector.Multiply(mel.NormSpeed, dx)) * Damping * dt * mel.NormSpeed / mel.NormSpeed.Length; // masse ignoriert
                                mel.NormSpeed += vdamp;
                            }
                            else
                            {
                                // Ground animator
                                Vector vdamp = -(Vector.Multiply(mel.NormSpeed, dx)) * Damping * dt * (dx / dx.Length); // masse ignoriert
                                mel.NormSpeed += vdamp;
                                if (mel.IsDeformable)
                                {
                                    mel.Deformation = new Rect(mel.Deformation.X, mel.Deformation.Y, mel.Deformation.Width + (dx.X * bend), mel.Deformation.Height + (dx.Y * bend));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dx.Length > 0)
                        {
                            me.Position += dx;
                            dx.Normalize();
                            me.NormSpeed -= -Math.Abs(Vector.Multiply(me.NormSpeed, dx)) * dx;
                            //Debug.WriteLine("## Inters V={0:##.0} dv={1:##.0}", dx, me.NormSpeed);
                        }
                    }  // RigidBody
                }
            }
            #endregion
        }


        public void Clear()
        {
            Obstacles.Clear();
        }
    }


}
