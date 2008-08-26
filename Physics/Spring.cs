using System;
using System.Drawing;

namespace Physics
{
    public class Spring : PhysicsObject
    {
        private const float densityPer100 = 9; // pocet "pruziniek" na 100px pri default width
        private const int defaultWidth = 10;

        
        public Sphere Sphere1, Sphere2;
        public float k, Length, Width;

        //float damping; %%%

        public Spring( World world )
        {
            this.world = world;
            ID = ++world.MaxID;

            Clr = Color.Black;
            Width = defaultWidth;
            //damping = 0.1f; %%%
        }

        public override void Tick( float time )
        {
            Vector deltaLocation = Sphere2.Location - Sphere1.Location;

            //if (deltaLocation.Abs() < length) return; // robi z pruziny gumu

            //Vector force = deltaLocation * ((k * (deltaLocation.Abs() - length)) / deltaLocation.Abs());
            Vector force = deltaLocation.Normalized() * k * (deltaLocation.Abs() - Length);

            Vector acceleration1 = force / Sphere1.Mass;
            Vector acceleration2 = (-1) * (force / Sphere2.Mass);

            //acceleration1 *= damping;%%%
            //acceleration2 *= damping;

            acceleration1 -= (world.AirFriction * Sphere1.Velocity.Abs()) * Sphere1.Velocity;
            acceleration2 -= (world.AirFriction * Sphere2.Velocity.Abs()) * Sphere2.Velocity;

            Sphere1.Velocity += time * acceleration1;
            Sphere2.Velocity += time * acceleration2;
        }

        public override void Render()
        {
            Graphics g = world.Graph;
            Color selectedClr = Clr;
            Pen p = new Pen( Clr, 1 );

            if (selected)
            {
                if (phase >= maxChange)
                    brighter = false;
                if (phase <= -maxChange)
                    brighter = true;

                int red = Clr.R;
                int green = Clr.G;
                int blue = Clr.B;

                if (red + 3 * phase > 255)
                    red = 255;
                else
                    if (red + 3 * phase < 0)
                        red = 0;
                    else
                        red = red + 3 * phase;

                if (green + 3 * phase > 255)
                    green = 255;
                else
                    if (green + 3 * phase < 0)
                        green = 0;
                    else
                        green = green + 3 * phase;

                if (blue + 3 * phase > 255)
                    blue = 255;
                else
                    if (blue + 3 * phase < 0)
                        blue = 0;
                    else
                        blue = blue + 3 * phase;

                if (brighter)
                    phase++;
                else
                    phase--;

                selectedClr = Color.FromArgb( red, green, blue );
                p.Color = selectedClr;
                p.Width = 2;
            }

            if (world.hqSprings)
            {
                float density = defaultWidth * densityPer100 / Width;

                int number = Convert.ToInt32( density * Length / 100 );

                if (number == 0)
                {
                    g.DrawLine( p, Sphere1.Location.x, Sphere1.Location.y, Sphere2.Location.x, Sphere2.Location.y );
                    return;
                }

                Line parallel1 = new Line( Sphere1.Location, Sphere2.Location );
                Line parallel2 = new Line( parallel1 );

                Vector shift = (Sphere2.Location - Sphere1.Location).Perpendicular();
                shift = shift.Normalized();
                shift = (Width / 2) * shift;

                if ((parallel1.End - parallel1.Start) * new Vector( 1, 0 ) < 0)
                    shift = -1 * shift;

                parallel1.Start = parallel1.Start + shift;
                parallel1.End = parallel1.End + shift;

                parallel2.Start = parallel2.Start - shift;
                parallel2.End = parallel2.End - shift;

                shift = Sphere2.Location - Sphere1.Location;
                shift = (1.0f / number) * shift;

                Vector drawLeft = parallel1.Start + (0.25f * shift);
                Vector drawRight = parallel2.Start + (0.75f * shift);

                g.DrawLine( p, Sphere1.Location.x, Sphere1.Location.y, drawLeft.x, drawLeft.y );

                for (int i = 0; i < number - 1; i++)
                {
                    g.DrawLine( p, drawLeft.x, drawLeft.y, drawRight.x, drawRight.y );
                    drawLeft += shift;
                    g.DrawLine( p, drawRight.x, drawRight.y, drawLeft.x, drawLeft.y );
                    drawRight += shift;
                }
                g.DrawLine( p, drawLeft.x, drawLeft.y, drawRight.x, drawRight.y );
                g.DrawLine( p, drawRight.x, drawRight.y, Sphere2.Location.x, Sphere2.Location.y );
            }
            else
            {
                g.DrawLine( p, Sphere1.Location.x, Sphere1.Location.y, Sphere2.Location.x, Sphere2.Location.y );
            }
        }

        public override string ToString()
        {
            string write = "SPR " +
                           Sphere1.ID.ToString() + " " +
                           Sphere2.ID.ToString() + " " +
                           k.ToString() + " " +
                           Length.ToString() + " " +
                           Width.ToString() + " " +
                           Clr.R.ToString() + " " + Clr.G.ToString() + " " + Clr.B.ToString() + " " +
                           ID.ToString();
            write = write.Replace( ',', '.' );
            return write;
        }

        public void FromFile( string info )
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                info = info.Replace( ".", "," );

            string[] s = info.Split( ' ' );

            long id1 = long.Parse( s[ 1 ] );
            long id2 = long.Parse( s[ 2 ] );

            foreach (Sphere sphere in world.spheres)
            {
                if (sphere.ID == id1)
                    Sphere1 = sphere;

                if (sphere.ID == id2)
                    Sphere2 = sphere;
            }

            k = float.Parse( s[ 3 ] );
            Length = float.Parse( s[ 4 ] );
            Width = float.Parse( s[ 5 ] );
            Clr = Color.FromArgb( int.Parse( s[ 6 ] ), int.Parse( s[ 7 ] ), int.Parse( s[ 8] ) );
            ID = long.Parse( s[ 9 ] );
        }

        public bool Connects( Sphere sphere )
        {
            if (sphere == Sphere1 || sphere == Sphere2)
                return true;
            return false;
        }

        public bool IsAtLocation( float x, float y )
        {
            Sphere tempSphere = new Sphere( null );
            tempSphere.Location.x = x;
            tempSphere.Location.y = y;
            tempSphere.Radius = Width;

            if (Geometry.Intersects( tempSphere, (new Line( Sphere1.Location, Sphere2.Location )) ))
                return true;
            return false;
        }
    }
}
