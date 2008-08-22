using System;
using System.Drawing;
using System.Collections.Generic;

namespace Physics
{
    public class World
    {
        public long MaxID;

        public List<Sphere> spheres;
        public List<Board> boards;
        public List<Spring> springs;
        private PhysicsObject selected;

        public Rectangle Bounds;
        public Vector Gravity;
        public float AirFriction;
        public bool Collisions;


        public Graphics Graph;

        // misc (appearance, performance) 
        // %%% nieco lepsie vymysliet
        public bool PrettySpheres;


        public const float G = 10000;

        public World( Graphics graphics, Rectangle bounds )
        {
            this.Graph = graphics;
            this.Bounds = bounds;

            MaxID = 0;
            Gravity = new Vector();
            AirFriction = 0f;
            Collisions = true;
            //objects = new List<PhysicsObject>();
            spheres = new List<Sphere>();
            springs = new List<Spring>();
            boards = new List<Board>();

            PrettySpheres = true; //%%%
        }

        public void Tick( float time )
        {
            foreach (Spring spring in springs)
            {
                spring.Tick( time );
            }

            foreach (Sphere sphere in spheres)
            {
                sphere.Tick( time );
            }

            if (Collisions)
            {
                for (int i = 0; i < spheres.Count - 1; ++i)
                {
                    for (int j = i + 1; j < spheres.Count; ++j)
                    {
                        spheresCollision( spheres[ i ], spheres[ j ] );
                    }
                }
            }
            //foreach (Sphere sphere1 in spheres)
            //{
            //    foreach (Sphere sphere2 in spheres)
            //    {
            //        //%%% NIE VSETKY DVOJICE
            //        resolveCollision( sphere1, sphere2 );
            //    }
            //}

            //%%% NIE VSETKY DVOJICE KOLIDUJ
        }

        public void Render()
        {
            foreach (Sphere sphere in spheres)
            {
                sphere.Render();
            }

            foreach (Spring spring in springs)
            {
                spring.Render();
            }

            foreach (Board board in boards)
            {
                board.Render();
            }
        }

        public override string ToString()
        {
            string write = "ENV " +
                           Gravity.x.ToString() + " " + Gravity.y.ToString() + " " +
                           AirFriction.ToString() + " " +
                           Collisions.ToString() + " " + 
                           MaxID.ToString();
            write = write.Replace( ',', '.' );
            return write;
        }

        public void FromFile( string info )
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                info = info.Replace( ".", "," );

            string[] s = info.Split( ' ' );

            Gravity.x = float.Parse( s[ 1 ] );
            Gravity.y = float.Parse( s[ 2 ] );
            AirFriction = float.Parse( s[ 3 ] );
            Collisions = bool.Parse( s[ 4 ] );
            MaxID = long.Parse( s[ 5 ] );
        }

        private static bool spheresCollision( Sphere sphere1, Sphere sphere2 )
        {
            //%%% STATIC
            //if (sphere1 == sphere2)
            //    return false;

            if (sphere1.Stationary && sphere2.Stationary)
                return false;

            if (sphere2.Stationary)
            {
                // Vymenim ich, aby som mal vzdy sphere1 stacionarnu, ak je jedna z nich stacionarna
                Sphere tmp = sphere2;
                sphere2 = sphere1;
                sphere1 = tmp;
            }

            Vector deltaVelocity = sphere1.Velocity - sphere2.Velocity;
            Vector deltaLocation = sphere2.Location - sphere1.Location;
            if (deltaLocation.Abs() < 1)
            {
                deltaLocation.x = 0.1f;
            }

            if (deltaLocation.Abs() < (sphere1.Radius + sphere2.Radius))
            {
                Vector deltaLocationNormalized = deltaLocation.Normalized();
                float m1 = sphere1.Mass;
                float m2 = sphere2.Mass;

                if (sphere1.Stationary)
                {
                    // rozdelim rychlost na kolme zlozky oproti osi kolizie
                    Vector vx1 = Vector.Projection( sphere2.Velocity, deltaLocationNormalized );
                    Vector vy1 = sphere2.Velocity - vx1;

                    Vector newVx1 = -sphere2.Elasticity * vx1;

                    sphere2.Velocity = newVx1 + vy1;
                }
                else
                {
                    if (Vector.ProjectionLength( deltaVelocity, deltaLocation ) > 0)
                    {
                        // zlozky rychlosti leziace na osi kolizie
                        Vector vx1 = Vector.Projection( sphere1.Velocity, deltaLocationNormalized );
                        Vector vx2 = Vector.Projection( sphere2.Velocity, deltaLocationNormalized );

                        // zlozky rychlosti leziace na kolmici na os kolizie
                        Vector vy1 = sphere1.Velocity - vx1;
                        Vector vy2 = sphere2.Velocity - vx2;

                        Vector newVx1 = (vx1 * (m1 - m2) + 2 * m2 * vx2) / (m1 + m2);
                        Vector newVx2 = (vx2 * (m2 - m1) + 2 * m1 * vx1) / (m1 + m2);

                        Vector deltaVelocity1 = (vy1 + newVx1) - sphere1.Velocity;
                        Vector deltaVelocity2 = (vy2 + newVx2) - sphere2.Velocity;

                        sphere1.Velocity += sphere1.Elasticity * deltaVelocity1;
                        sphere2.Velocity += sphere2.Elasticity * deltaVelocity2;
                    }
                }

                // ---------movenutie guli von zo seba------------
                float move = (sphere1.Radius + sphere2.Radius) - deltaLocation.Abs();

                if (sphere1.Stationary)
                {
                    Vector move1 = move * deltaLocationNormalized;
                    sphere2.Location += move1;
                }
                else
                {
                    float p1 = (m2 / (m1 + m2)) * move;
                    float p2 = move - p1;
                    Vector move1 = -p1 * deltaLocationNormalized;
                    Vector move2 = p2 * deltaLocationNormalized;
                    sphere1.Location += move1;
                    sphere2.Location += move2;
                }
                return true;
            }
            return false;
        }

        public void AddSphere( Sphere sphere )
        {
            sphere.Render();
            spheres.Add( sphere );
        }

        public void AddSphere( float x, float y, float vx, float vy, int radius, float mass, float elasticity, float gravityStrength, Color color, bool stationary )
        {
            Sphere sphere = new Sphere( this );
            sphere.Location.x = x;
            sphere.Location.y = y;
            sphere.Velocity.x = vx;
            sphere.Velocity.y = vy;
            //Random r = new Random();
            //sphere.Color = Color.FromArgb( r.Next( 256 ), r.Next( 256 ), r.Next( 256 ) );
            sphere.Mass = mass;
            sphere.Elasticity = elasticity;
            sphere.Radius = radius;
            sphere.GravityStrength = gravityStrength;
            sphere.Clr = color;
            sphere.Stationary = stationary;

            sphere.Render();
            spheres.Add( sphere );
        }

        public void AddSpring( Spring spring )
        {
            spring.Render();
            springs.Add( spring );
        }

        public void AddSpring( Sphere sphere1, Sphere sphere2, float k, float length )
        {
            Spring spring = new Spring( this );

            spring.Sphere1 = sphere1;
            spring.Sphere2 = sphere2;
            spring.k = k;
            spring.Length = length;

            springs.Add( spring );
        }

        public void AddBoard( Board board )
        {
            board.Render();
            boards.Add( board );
        }

        public void AddBoard( float x1, float y1, float x2, float y2 )
        {
            Board board = new Board( this );
            board.line.Start.x = x1;
            board.line.Start.y = y1;
            board.line.End.x = x2;
            board.line.End.y = y2;

            board.Render();
            boards.Add( board );
        }

        public void ChangeLocationOfSpheres( int dx, int dy )
        {
            foreach (Sphere sphere in spheres)
            {
                sphere.Location.x += dx;
                sphere.Location.y += dy;
            }
        }

        public PhysicsObject SelectObject( float x, float y )
        {
            foreach (Sphere sphere in spheres)
            {
                if (sphere.IsAtLocation( x, y ))
                {
                    if (selected != null)
                        selected.Deselect();
                    selected = sphere;
                    sphere.Select();
                    return sphere;
                }
            }

            foreach (Board board in boards)
            {
                if (board.IsAtLocation( x, y ))
                {
                    if (selected != null)
                        selected.Deselect();
                    selected = board;
                    board.Select();
                    return board;
                }
            }

            foreach (Spring spring in springs)
            {
                if (spring.IsAtLocation( x, y ))
                {
                    if (selected != null)
                        selected.Deselect();
                    selected = spring;
                    spring.Select();
                    return spring;
                }
            }

            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            return null;
        }

        public void DeleteSelected()
        {
            if (selected != null)
            {
                if (selected is Sphere)
                {
                    Sphere delete = selected as Sphere;
                    foreach (Spring spring in springs)
                    {
                        if (spring.Connects( delete ))
                        {
                            springs.Remove( spring );
                        }
                    }
                    spheres.Remove( delete );
                }

                if (selected is Board)
                {
                    boards.Remove( selected as Board );
                }

                if (selected is Spring)
                {
                    springs.Remove( selected as Spring );
                }

                selected = null;
            }
        }

        public void MoveSelected( Vector delta )
        {
            if (selected != null)
            {
                if (selected is Sphere)
                {
                    (selected as Sphere).Location += delta;
                }

                if (selected is Board)
                {
                    (selected as Board).line.Start += delta;
                    (selected as Board).line.End += delta;
                }
            }
        }

        public int SpheresCount()
        {
            return spheres.Count;
        }

        public void ClearSpheres()
        {
            springs.Clear();
            spheres.Clear();
        }

        public void ClearBoards()
        {
            boards.Clear();
        }

        public void ClearSprings()
        {
            springs.Clear();
        }

        public void ClearScene()
        {
            springs.Clear();
            spheres.Clear();
            boards.Clear();
            MaxID = 0;
        }

        public void SaveScene( string path )
        {
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter( path );

            streamWriter.WriteLine( this.ToString() );

            foreach (Sphere sphere in spheres)
            {
                streamWriter.WriteLine( sphere.ToString() );
            }

            foreach (Spring spring in springs)
            {
                streamWriter.WriteLine( spring.ToString() );
            }

            foreach (Board board in boards)
            {
                streamWriter.WriteLine( board.ToString() );
            }

            streamWriter.Close();
        }

        public void LoadScene( string path )
        {
            this.ClearScene();

            System.IO.StreamReader streamReader = new System.IO.StreamReader( path );

            string read;

            while (!streamReader.EndOfStream)
            {
                read = streamReader.ReadLine();

                switch (read.Substring( 0, 3 ))
                {
                case "ENV":
                    this.FromFile( read );
                    break;
                case "SPH":
                    Sphere sphere = new Sphere( this );
                    sphere.FromFile( read );
                    sphere.Render();
                    spheres.Add( sphere );
                    break;
                case "SPR":
                    Spring spring = new Spring( this );
                    spring.FromFile( read );
                    spring.Render();
                    springs.Add( spring );
                    break;
                case "BOA":
                    Board board = new Board( this );
                    board.FromFile( read );
                    board.Render();
                    boards.Add( board );
                    break;
                }
            }
        }
    }
}
