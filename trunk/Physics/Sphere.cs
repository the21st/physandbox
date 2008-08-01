using System;
using System.Drawing;
using System.Collections.Generic;

namespace Physics
{
    public class Sphere : PhysicsObject
    {
        private const float edgeWidth = 0.1f; //pomer okraju gule voci polomeru

        public Vector Location, Velocity;
        public float Radius, Mass, Elasticity, GravityStrength;
        public Color Clr;
        public bool Stationary;

        public Sphere( World world )
        {
            this.world = world;

            Location = new Vector();
            Velocity = new Vector();
            Radius = 10;
            Mass = 1;
            Elasticity = 1;
            Clr = Color.Black;
            Stationary = false;
            GravityStrength = 0;
        }

        public override void Tick( float time )
        {
            if (!this.Stationary)
            {
                Vector acceleration = world.Gravity;

                acceleration -= (world.AirFriction * Velocity.Abs()) * Velocity;
                Velocity += time * acceleration;

                //Location += time * Velocity;
                this.move( time );

                this.keepInBounds( world.Bounds ); //%%% sem dorobit steny on/off

                this.applyGravity( time ); //%%% na zaciatku alebo na konci?
            }
        }

        public override void Render()
        {
            Graphics g = world.Graph;
            if (world.PrettySpheres)
            {
                SolidBrush b = new SolidBrush( Clr );

                int red = Clr.R;
                int green = Clr.G;
                int blue = Clr.B;
                red = red - 50 < 0 ? 0 : red - 50;
                green = green - 50 < 0 ? 0 : green - 50;
                blue = blue - 50 < 0 ? 0 : blue - 50;
                Color c = Color.FromArgb( red, green, blue );
                Pen p = new Pen( c );
                p.Width = Radius * edgeWidth < 1 ? 1 : Radius * edgeWidth;

                Sphere tmp = new Sphere( null );
                tmp.Location = Location;
                tmp.Radius = Radius - p.Width / 2;

                g.FillEllipse( b, this.GetRectangle() );
                g.DrawEllipse( p, tmp.GetRectangle() );

                p.Dispose();
                b.Dispose();
            }
            else
            {
                Pen p = new Pen( Clr );
                g.DrawEllipse( p, this.GetRectangle() );
                p.Dispose();
            }
        }

        private void applyGravity( float time )
        {
            List<Sphere> spheres = world.spheres;
            foreach (Sphere sphere in spheres)
            {
                if (sphere != this && this.GravityStrength != 0 && !sphere.Stationary)
                {
                    Vector deltaLocation = this.Location - sphere.Location;
                    if (deltaLocation.Abs() < 1)
                        return;

                    float r = deltaLocation.Abs();

                    Vector force = (World.G * this.GravityStrength * sphere.Mass / (r * r)) * deltaLocation.Normalized();
                    //Vector force = (World.G * this.GravityStrength / (r * r)) * deltaLocation.Normalized(); // zahada: ktore?%%%

                    Vector acceleration = force / sphere.Mass;

                    acceleration -= (world.AirFriction * sphere.Velocity.Abs()) * sphere.Velocity;

                    sphere.Velocity += time * acceleration;
                }
            }
        }

        private bool keepInBounds( Rectangle bounds )
        {
            // odrazanie od stien so zakonom zachovania energie (kin+pot) %%%

            // -------STROP-------
            if (Location.y - Radius < bounds.Top)
            {
                if (Velocity.y < 0)
                    Velocity.y = -Velocity.y * Elasticity;
                Velocity.x = 0.95f * Velocity.x; // "trenie"
                Location.y = bounds.Top + Radius;
            }

            // -------PODLAHA--------
            if (Location.y + Radius > bounds.Bottom)
            {
                if (Velocity.y > 0)
                    Velocity.y = -Velocity.y * Elasticity;
                Velocity.x = 0.95f * Velocity.x; // "trenie"
                Location.y = bounds.Bottom - Radius;
            }

            // -------LAVA STENA------
            if (Location.x - Radius < bounds.Left)
            {
                if (Velocity.x < 0)
                    Velocity.x = -Velocity.x * Elasticity;
                Location.x = Radius;
            }

            // -------PRAVA STENA------
            if (Location.x + Radius > bounds.Right)
            {
                if (Velocity.x > 0)
                    Velocity.x = -Velocity.x * Elasticity;
                Location.x = bounds.Right - Radius;
            }

            return false;
        }

        private void move( float time )
        {
            List<Board> boards = world.boards;

            //%%% pokracuj


            Location += time * Velocity;
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle( Convert.ToInt32( Location.x - Radius ), Convert.ToInt32( Location.y - Radius ), Convert.ToInt32( 2 * Radius ), Convert.ToInt32( 2 * Radius ) );
        }

        public bool IsAtLocation( float x, float y )
        {
            Vector dist = new Vector( x, y ) - this.Location;

            if (dist.Abs() < this.Radius)
                return true;
            return false;
        }
    }
}
