using System;
using System.Drawing;
using System.Collections.Generic;

namespace Physics
{
    /// <summary>
    /// Trieda, ktora reprezentuje 2D gulu (kruh) ako fyzikalny objekt.
    /// </summary>
    public class Sphere : PhysicsObject
    {
        private const float edgeWidth = 0.1f; //pomer okraju gule voci polomeru

        public Vector Location, Velocity;
        public float Radius, Mass, Elasticity, GravityStrength;
        public bool Stationary;

        public Sphere( World world )
        {
            if (world != null)
            {
                this.world = world;
                ID = ++world.MaxID;
            }

            Location = new Vector();
            Velocity = new Vector();
            Radius = 10;
            Mass = 1;
            Elasticity = 1;
            Clr = Color.Black;
            Stationary = false;
            GravityStrength = 0;
        }

        /// <summary>
        /// Metoda, ktora posunie gulu v zadanom case tak, ako to urcuju jej vlastnosti a vlastnosti World-u.
        /// </summary>
        public override void Tick( float time )
        {
            if (!this.Stationary)
            {
                Vector acceleration = world.Gravity;

                acceleration -= (world.AirFriction * Velocity.Abs()) * Velocity; // aplikacia trenia vzduchu
                Velocity += time * acceleration;

                this.move( time );

                this.resolveOverlapping();

                if (world.Walls)
                    this.keepInBounds( world.Bounds );
            }

            this.applyGravity( time );
        }

        /// <summary>
        /// Vykresli gulu na platno World-u, ktoremu gula prislucha.
        /// </summary>
        public override void Render()
        {
            Graphics g = world.Graph;
            Color selectedClr = Clr;

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
            }

            if (world.hqSpheres)
            {
                SolidBrush b = new SolidBrush( selectedClr );

                int red = selectedClr.R;
                int green = selectedClr.G;
                int blue = selectedClr.B;
                red = red - 50 < 0 ? 0 : red - 50;
                green = green - 50 < 0 ? 0 : green - 50;
                blue = blue - 50 < 0 ? 0 : blue - 50;
                Pen p = new Pen( Color.FromArgb( red, green, blue ) );
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
                Pen p = new Pen( selectedClr );
                g.DrawEllipse( p, this.GetRectangle() );
                p.Dispose();
            }
        }

        /// <summary>
        /// Ak ma tato gula gravitacnu silu, tak ju postupne aplikuje na vsetky ostatne gule.
        /// </summary>
        private void applyGravity( float time )
        {
            if (this.GravityStrength == 0)
                return;

            List<Sphere> spheres = world.spheres;
            foreach (Sphere sphere in spheres)
            {
                if (sphere != this && !sphere.Stationary)
                {
                    Vector deltaLocation = this.Location - sphere.Location;
                    if (deltaLocation.Abs() > 1)  // ak su od seba gule vzdialene menej ako jeden pixel, sila nebude aplikovana (bola by prilis velka)
                    {
                        float r = deltaLocation.Abs();

                        // podla newtonovho gravitacneho zakona:
                        Vector force = (World.G * this.GravityStrength * sphere.Mass / (r * r)) * deltaLocation.Normalized();

                        Vector acceleration = force / sphere.Mass;

                        acceleration -= (world.AirFriction * sphere.Velocity.Abs()) * sphere.Velocity; // trenie vzduchu

                        sphere.Velocity += time * acceleration;
                    }
                }
            }
        }

        /// <summary>
        /// Udrzuje gulu vovnutri hranic sveta.
        /// </summary>
        private void keepInBounds( Rectangle bounds )
        {
            // dorobit odrazanie od stien so zakonom zachovania energie (kinetickej + potencialnej) %%

            // vylepsit "trenie" pouzite v tejto metode %%

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
                Velocity.y = 0.95f * Velocity.y; // "trenie"
                Location.x = Radius;
            }

            // -------PRAVA STENA------
            if (Location.x + Radius > bounds.Right)
            {
                if (Velocity.x > 0)
                    Velocity.x = -Velocity.x * Elasticity;
                Velocity.y = 0.95f * Velocity.y; // "trenie"
                Location.x = bounds.Right - Radius;
            }
        }

        /// <summary>
        /// Posuva gulu tak, aby sa neprekryvala s doskami.
        /// </summary>
        private void resolveOverlapping()
        {
            List<Board> boards = world.boards;

            foreach (Board board in boards)
            {
                Vector v = Geometry.Overlap( this, board );
                if (v != null)
                    this.Location = v;
            }
        }

        /// <summary>
        /// Na zaklade polohy a rychlosti gule skontroluje ci v jej drahe pohybu prekaza nejaka doska.
        /// Ak ano, spracuje koliziu a odraz. Ak nie, jednoducho pohne gulou na jej miesto (pozicia+rychlost).
        /// </summary>
        private void move( float time )
        {
            List<Board> boards = world.boards;
            List<CollisionInfo> collisions = new List<CollisionInfo>(); // zoznam vsetkych kolizii, ktore by mohli nastat v drahe pohybu gule

            foreach (Board board in boards)
            {
                // naplnanie zoznamu kolizii
                CollisionInfo collision = Geometry.Collision( this, board, time );
                if (collision != null)
                    collisions.Add( collision );
            }

            // zistenie, ktora z kolizii je najblizsia, a teda by nastala prva, ak by bol pohyb gule kontinualny (spojity)
            CollisionInfo closest = null;

            if (collisions.Count > 0)
            {
                float min = int.MaxValue;
                float dist;

                foreach (CollisionInfo ci in collisions)
                {
                    Vector v = ci.Location;
                    dist = (v - this.Location).Abs();
                    if (dist < min)
                    {
                        min = dist;
                        closest = ci;
                    }
                }
            }
            else
            {
                Location += time * Velocity;
                return;
            }

            Board b = closest.With; // b je doska, s ktorou nastala kolizia

            if (Geometry.Overlap( this, b ) == null)
            {
                if (closest.Type == 1) // to znamena, ze gula narazi do usecky samotnej, a nie do jednej z jej koncovych bodov
                {
                    Vector vx1 = Vector.Projection( this.Velocity, b.line.End - b.line.Start );
                    Vector vy1 = this.Velocity - vx1;

                    Vector newVy1 = -this.Elasticity * vy1;

                    this.Velocity = newVy1 + vx1;

                    Vector correction = (b.line.End - b.line.Start).Perpendicular().Normalized();
                    correction = 0.1f * correction;

                    Vector temp1 = closest.Location + correction;
                    Vector temp2 = closest.Location - correction;

                    Vector newLocation;
                    if ((b.line.Start - temp1).Abs() > (b.line.Start - temp2).Abs())
                        newLocation = temp1;
                    else
                        newLocation = temp2;

                    this.Location = newLocation;
                }

                if (closest.Type == 2) // to znamena, ze gula narazi do jedneho z koncovych bodov usecky
                {
                    Vector p1 = b.line.Start;
                    Vector p2 = b.line.End;
                    Vector pointOfCollision;

                    if ((closest.Location - p1).Abs() < (closest.Location - p2).Abs())
                        pointOfCollision = p1;
                    else
                        pointOfCollision = p2;

                    Vector temp = pointOfCollision - closest.Location;

                    Vector vx1 = Vector.Projection( this.Velocity, temp );
                    Vector vy1 = this.Velocity - vx1;

                    Vector newVx1 = -this.Elasticity * vx1;

                    this.Velocity = newVx1 + vy1;

                    Vector correction = (closest.Location - pointOfCollision).Normalized();
                    correction = 0.1f * correction;

                    Vector newLocation = closest.Location + correction;
                    this.Location = newLocation;
                }
            }
        }

        /// <summary>
        /// Vrati Rectangle (v skutocnosti to vzdy bude stvorec) ktoremu je gula vpisana.
        /// Pouziva sa najma pri vykreslovani gule.
        /// </summary>
        public Rectangle GetRectangle()
        {
            try
            {
                return new Rectangle( Convert.ToInt32( Location.x - Radius ), Convert.ToInt32( Location.y - Radius ), Convert.ToInt32( 2 * Radius ), Convert.ToInt32( 2 * Radius ) );
            }
            catch (OverflowException e)
            {
                // toto sa obcas, ked gula odleti prilis daleko, stane. ale vtedy ju aj tak netreba vykreslit, cize to nevadi
            }
            return new Rectangle( -1, -1, -1, -1 );
        }

        /// <summary>
        /// Zisti, ci sa gula nachadza v danych suradniciach (
        /// </summary>
        public bool IsAtLocation( float x, float y )
        {
            Vector dist = new Vector( x, y ) - this.Location;

            if (dist.Abs() < this.Radius)
                return true;
            return false;
        }

        /// <summary>
        /// Nacita vlastnosti gule zo stringu, ktory zodpoveda jej textovemu zapisu.
        /// </summary>
        public void FromFile( string info )
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                info = info.Replace( ".", "," );

            string[] s = info.Split( ' ' );

            Location.x = float.Parse( s[ 1 ] );
            Location.y = float.Parse( s[ 2 ] );
            Velocity.x = float.Parse( s[ 3 ] );
            Velocity.y = float.Parse( s[ 4 ] );
            Radius = float.Parse( s[ 5 ] );
            Mass = float.Parse( s[ 6 ] );
            Elasticity = float.Parse( s[ 7 ] );
            GravityStrength = float.Parse( s[ 8 ] );
            Stationary = bool.Parse( s[ 9 ] );
            Clr = Color.FromArgb( int.Parse( s[ 10 ] ), int.Parse( s[ 11 ] ), int.Parse( s[ 12 ] ) );
            ID = long.Parse( s[ 13 ] );
        }

        public override string ToString()
        {
            string write = "SPH " +
                           Location.x.ToString() + " " + Location.y.ToString() + " " +
                           Velocity.x.ToString() + " " + Velocity.y.ToString() + " " +
                           Radius.ToString() + " " + Mass.ToString() + " " +
                           Elasticity.ToString() + " " + GravityStrength.ToString() + " " +
                           Stationary.ToString() + " " +
                           Clr.R.ToString() + " " + Clr.G.ToString() + " " + Clr.B.ToString() + " " +
                           ID.ToString();
            write = write.Replace( ',', '.' );
            return write;
        }
    }
}
