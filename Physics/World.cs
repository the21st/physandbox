using System;
using System.Drawing;
using System.Collections.Generic;

namespace Physics
{
    public class World
    {
        //private List<PhysicsObject> objects;
        private List<Sphere> spheres;
        private List<Spring> springs;
        private List<Board> boards;

        public Rectangle Bounds;
        public Vector Gravity;
        public float AirFriction;
        public Graphics Graph;
        public bool Collisions;

        // misc (appearance, performance) 
        // %%% nieco lepsie vymysliet
        public bool PrettySpheres;


        public const float G = 10000;

        public World( Graphics graphics, Rectangle bounds )
        {
            this.Graph = graphics;
            this.Bounds = bounds;

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
            //%%%
            //foreach (Sphere sphere in spheres)
            //{
            //    if (sphere.GravityStrength != 0)
            //    {
            //        sphere.ApplyGravity( spheres );
            //    }
            //}

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
                        resolveCollision( spheres[ i ], spheres[ j ] );
                    }
                }
            }

            foreach (Sphere sphere in spheres)
            {
                if (sphere.GravityStrength != 0)
                {
                    sphere.ApplyGravity( time, spheres );
                }
            }

            foreach (Spring spring in springs)
            {
                spring.Tick( time );
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
        }

        public void ChangeLocationOfSpheres( int dx, int dy )
        {
            foreach (Sphere sphere in spheres)
            {
                sphere.Location.x += dx;
                sphere.Location.y += dy;
            }
        }

        public int SpheresCount()
        {
            return spheres.Count;
        }

        public void ClearSpheres()
        {
            spheres.Clear();
        }

        public void ClearAll()
        {
            springs.Clear();
            spheres.Clear();
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

        public void AddSpring( Sphere sphere1, Sphere sphere2, float k, float length )
        {
            Spring spring = new Spring( this, sphere1, sphere2, k, length );

            springs.Add( spring );
        }

        public void AddBoard( float x1, float y1, float x2, float y2 )
        {
            Board board = new Board( this );
            board.Start.x = x1;
            board.Start.y = y1;
            board.End.x = x2;
            board.End.y = y2;
        }

        private static bool resolveCollision( PhysicsObject obj1, PhysicsObject obj2 )
        {
            if (obj1 != obj2)
            {

                if (obj1 is Sphere)
                {
                    Sphere sphere1 = obj1 as Sphere;
                    if (obj2 is Sphere)
                    {
                        Sphere sphere2 = obj2 as Sphere;
                        return spheresCollision( sphere1, sphere2 );
                    }

                    // if (obj2 is Board) %%%
                }
                else
                {
                    //if (obj1 is Board) %%%
                    //{
                    //    if (obj2 is Sphere)
                    //    {
                    //        Sphere sphere1 = obj2 as Sphere;
                    //        Board Board = obj1 as Board;
                    //        return sphereReflectorCollision( sphere1, Board );
                    //    }
                    //}
                }
            }
            return false;
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

                // ---------moveutie guli von zo seba------------
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

        private static bool sphereBoardCollision( Sphere sphere, Board board )
        {
            // %%% nakresy, rovnobezky, hladanie kruznic, geometria
            // jednoducha (matusova) vs. zlozita (snad bude) verzia
            return false;
        }
    }
}
