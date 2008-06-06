using System;
using System.Drawing;

namespace Physics
{
    public class Spring : PhysicsObject
    {
        private const float density = 0;

        public Color Clr;

        Sphere sphere1, sphere2;
        float length;
        float k;
        float width;
        float lengthN;
        //float damping; %%%

        public Spring( World world, Sphere sphere1, Sphere sphere2, float k, float length )
        {
            this.world = world;
            this.sphere1 = sphere1;
            this.sphere2 = sphere2;
            this.k = k;
            this.length = length;
            Clr = Color.Black;

            width = 30;
            lengthN = 30;
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

            //int number = Convert.ToInt32(length / lengthN);

            //for (int i = 0; i < number)
            //{

            //}
            //// %%% dorobit peknu pruzinu, to bude tazsie

            g.DrawLine( p, sphere1.Location.x, sphere1.Location.y, sphere2.Location.x, sphere2.Location.y );
        }
    }
}
