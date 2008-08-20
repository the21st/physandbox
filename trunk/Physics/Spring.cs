using System;
using System.Drawing;

namespace Physics
{
    public class Spring : PhysicsObject
    {
        private const float densityPer100 = 9; // pocet "pruziniek" na 100px pri default width
        private const int defaultWidth = 10;

        public Color Clr;
        float width;

        Sphere sphere1, sphere2;
        float length;
        float k;

        //float damping; %%%

        public Spring( World world, Sphere sphere1, Sphere sphere2, float k, float length )
        {
            this.world = world;
            this.sphere1 = sphere1;
            this.sphere2 = sphere2;
            this.k = k;
            this.length = length;
            Clr = Color.Black;

            width = defaultWidth;
            //damping = 0.1f; %%%
        }

        public override void Tick( float time )
        {
            Vector deltaLocation = sphere2.Location - sphere1.Location;

            //if (deltaLocation.Abs() < length) return; // robi z pruziny gumu

            //Vector force = deltaLocation * ((k * (deltaLocation.Abs() - length)) / deltaLocation.Abs());
            Vector force = deltaLocation.Normalized() * k * (deltaLocation.Abs() - length);

            Vector acceleration1 = force / sphere1.Mass;
            Vector acceleration2 = (-1) * (force / sphere2.Mass);

            //acceleration1 *= damping;%%%
            //acceleration2 *= damping;

            acceleration1 -= (world.AirFriction * sphere1.Velocity.Abs()) * sphere1.Velocity;
            acceleration2 -= (world.AirFriction * sphere2.Velocity.Abs()) * sphere2.Velocity;

            sphere1.Velocity += time * acceleration1;
            sphere2.Velocity += time * acceleration2;
        }

        public override void Render()
        {
            Graphics g = world.Graph;
            Pen p = new Pen( this.Clr );

            if (world.PrettySpheres) //%%% zmenit
            {
                float density = defaultWidth * densityPer100 / width;

                int number = Convert.ToInt32( density * length / 100 );

                Line parallel1 = new Line( sphere1.Location, sphere2.Location );
                Line parallel2 = new Line( parallel1 );

                Vector shift = (sphere2.Location - sphere1.Location).Perpendicular();
                shift = shift.Normalized();
                shift = (width / 2) * shift;

                parallel1.Start = parallel1.Start + shift;
                parallel1.End = parallel1.End + shift;

                parallel2.Start = parallel2.Start - shift;
                parallel2.End = parallel2.End - shift;

                shift = sphere2.Location - sphere1.Location;
                shift = (1.0f / number) * shift;

                Vector drawLeft = parallel1.Start + (0.25f * shift);
                Vector drawRight = parallel2.Start + (0.75f * shift);

                g.DrawLine( p, sphere1.Location.x, sphere1.Location.y, drawLeft.x, drawLeft.y );

                for (int i = 0; i < number - 1; i++)
                {
                    g.DrawLine( p, drawLeft.x, drawLeft.y, drawRight.x, drawRight.y );
                    drawLeft += shift;
                    g.DrawLine( p, drawRight.x, drawRight.y, drawLeft.x, drawLeft.y );
                    drawRight += shift;
                }
                g.DrawLine( p, drawLeft.x, drawLeft.y, drawRight.x, drawRight.y );
                g.DrawLine( p, drawRight.x, drawRight.y, sphere2.Location.x, sphere2.Location.y );
                // %%% dorobit peknu pruzinu, to bude tazsie
            }
            else
            {
                g.DrawLine( p, sphere1.Location.x, sphere1.Location.y, sphere2.Location.x, sphere2.Location.y );
            }
        }
    }
}
