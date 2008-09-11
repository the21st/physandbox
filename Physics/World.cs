using System;
using System.Drawing;
using System.Collections.Generic;

namespace Physics
{
    /// <summary>
    /// Trieda, ktora je v hierarchii kniznice physics najvyssie.
    /// Obsahuje vacsinu ovladacich prvkov, pomocou nej sa naraba
    /// s celym fyzikalnym svetom, ktory je spracovavany.
    /// </summary>
    public class World
    {
        public const float G = 10000;     // konstanta pouzita pri aplikacii newtonovho gravitacneho zakona (nezodpoveda skutocnej hodnote)


        public Graphics Graph;            // Grafika (platno) na ktore sa kresli svet
        public long MaxID;                // zatial maximalne rozdane unikatne ID cislo

        public List<Sphere> spheres;      // zoznam gul
        public List<Board> boards;        // zoznam dosiek
        public List<Spring> springs;      // zoznam pruzin
        public PhysicsObject Selected;    // pointer na objekt ktory je prave oznaceny

        public Rectangle Bounds;          // hranice (steny) sveta
        public Vector Gravity;            // smer a sila homogennej gravitacnej sily sveta
        public float AirFriction;         // sila trenia vzduchu
        public bool Collisions, Walls;    // zapnute-vypnute kolizie gul a hranice sveta

        public bool hqSpheres, hqSprings; // tykaju sa vizualnej stranky - urcuju kvalitu vykreslenie gul a pruzin


        public World( Graphics graphics, Rectangle bounds )
        {
            this.Graph = graphics;
            this.Bounds = bounds;

            MaxID = 0;
            Gravity = new Vector( 0, 1 );
            AirFriction = 0.0005f;
            Collisions = true;
            spheres = new List<Sphere>();
            springs = new List<Spring>();
            boards = new List<Board>();

            Collisions = true;
            Walls = true;

            hqSpheres = true;
            hqSprings = true;
        }

        /// <summary>
        /// Metoda, ktora pohne celym svetom o zadanu casovu jednotku.
        /// </summary>
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
            // optimalizacia - nekontrolovat vsetky dvojice? %%
        }

        /// <summary>
        /// metoda ktora vykresli vsetko, co treba na platno.
        /// </summary>
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

        /// <summary>
        /// Metoda, ktora sa postara o koliziu dvoch sfer. 
        /// Zisti, ci koliduju (koliduju ak sa v danom momente prekryvaju), a ak ano,
        /// zmeni ich rychlosti a posunie ich von zo seba.
        /// </summary>
        private static bool spheresCollision( Sphere sphere1, Sphere sphere2 )
        {
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
                // toto je taky trochu "podvod", ktorym dosiahnem, ze pri kolizii nikdy nepracujem s dvomi gulami, ako keby boli na tom istom mieste, aj ked su.
            }

            if (deltaLocation.Abs() < (sphere1.Radius + sphere2.Radius)) // kontrola, ci sa sfery prekryvaju, tj. ci koliduju
            {
                // vacsina nasledujuceho kodu je fyzika kolizii prepisana do kodu

                Vector deltaLocationNormalized = deltaLocation.Normalized();
                float m1 = sphere1.Mass;
                float m2 = sphere2.Mass;

                if (sphere1.Stationary)
                {
                    // rozdelim rychlost na kolme zlozky voci osi kolizie
                    Vector vx1 = Vector.Projection( sphere2.Velocity, deltaLocationNormalized );
                    Vector vy1 = sphere2.Velocity - vx1;

                    Vector newVx1 = -sphere2.Elasticity * vx1;

                    sphere2.Velocity = newVx1 + vy1;
                }
                else
                {
                    // kontrola, ci sa skutocne v aktualnom momente sfery hybu oproti sebe
                    // moze sa stat, ze nie - ak sa sfery hybu prilis rychlo / casovy skok je prilis velky
                    // a sfery cez seba "preskocia", ale stale sa prekryvaju
                    if (Vector.ProjectionLength( deltaVelocity, deltaLocation ) > 0)
                    {
                        // zlozky rychlosti leziace na osi kolizie
                        Vector vx1 = Vector.Projection( sphere1.Velocity, deltaLocationNormalized );
                        Vector vx2 = Vector.Projection( sphere2.Velocity, deltaLocationNormalized );

                        // zlozky rychlosti leziace na kolmici na os kolizie
                        Vector vy1 = sphere1.Velocity - vx1;
                        Vector vy2 = sphere2.Velocity - vx2;

                        // vzorec
                        Vector newVx1 = (vx1 * (m1 - m2) + 2 * m2 * vx2) / (m1 + m2);
                        Vector newVx2 = (vx2 * (m2 - m1) + 2 * m1 * vx1) / (m1 + m2);

                        Vector deltaVelocity1 = (vy1 + newVx1) - sphere1.Velocity;
                        Vector deltaVelocity2 = (vy2 + newVx2) - sphere2.Velocity;

                        sphere1.Velocity += sphere1.Elasticity * deltaVelocity1;
                        sphere2.Velocity += sphere2.Elasticity * deltaVelocity2;
                    }
                }

                // posunutie guli von zo seba
                float move = (sphere1.Radius + sphere2.Radius) - deltaLocation.Abs();

                if (sphere1.Stationary)
                {
                    Vector move1 = move * deltaLocationNormalized;
                    sphere2.Location += move1;
                }
                else
                {
                    // tazsiu gulu posuvam menej ako lahsiu:
                    float p1 = (m2 / (m1 + m2)) * move; // vypocitanie pomeru
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
            spheres.Add( sphere );
        }

        public void AddSpring( Spring spring )
        {
            springs.Add( spring );
        }

        public void AddBoard( Board board )
        {
            boards.Add( board );
        }

        public PhysicsObject SelectObject( float x, float y )
        {
            // tu sa spytam kazdeho objektu ci sa nachadza na danych suradniciach

            foreach (Sphere sphere in spheres)
            {
                if (sphere.IsAtLocation( x, y ))
                {
                    if (Selected != null)
                        Selected.Deselect();
                    Selected = sphere;
                    sphere.Select();
                    return sphere;
                }
            }

            foreach (Board board in boards)
            {
                if (board.IsAtLocation( x, y ))
                {
                    if (Selected != null)
                        Selected.Deselect();
                    Selected = board;
                    board.Select();
                    return board;
                }
            }

            foreach (Spring spring in springs)
            {
                if (spring.IsAtLocation( x, y ))
                {
                    if (Selected != null)
                        Selected.Deselect();
                    Selected = spring;
                    spring.Select();
                    return spring;
                }
            }

            if (Selected != null)
            {
                Selected.Deselect();
                Selected = null;
            }
            return null;
        }

        public Sphere SelectSphere( float x, float y )
        {
            // to iste ako SelectObject, akurat berie len sfery

            foreach (Sphere sphere in spheres)
            {
                if (sphere.IsAtLocation( x, y ))
                {
                    if (Selected != null)
                        Selected.Deselect();
                    Selected = sphere;
                    sphere.Select();
                    return sphere;
                }
            }

            return null;
        }

        public void Deselect()
        {
            if (Selected != null)
            {
                Selected.Deselect();
                Selected = null;
            }
        }

        public void DeleteSelected()
        {
            // co je oznacene removnem zo zoznamu objektov

            if (Selected != null)
            {
                if (Selected is Sphere)
                {
                    // jediny problem je, ak vymazavam gulu - vtedy musim zmazat aj pruziny ktore su o nu pripevnene

                    Sphere delete = Selected as Sphere;
                    for (int i = springs.Count - 1; i >= 0; --i)
                    {
                        if (springs[ i ].Connects( delete ))
                        {
                            springs.RemoveAt( i );
                        }
                    }
                    spheres.Remove( delete );
                }

                if (Selected is Board)
                {
                    boards.Remove( Selected as Board );
                }

                if (Selected is Spring)
                {
                    springs.Remove( Selected as Spring );
                }

                Selected = null;
            }
        }

        public void MoveSelected( Vector delta )
        {
            if (Selected != null)
            {
                if (Selected is Sphere)
                {
                    (Selected as Sphere).Location += delta;
                }

                if (Selected is Board)
                {
                    (Selected as Board).line.Start += delta;
                    (Selected as Board).line.End += delta;
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

        /// <summary>
        /// Ulozi celu scenu aj s nastaveniami environmentu do suboru specifikovaneho parametrom.
        /// </summary>
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

        /// <summary>
        /// Nacita celu scenu aj s nastaveniami environmentu zo suboru.
        /// </summary>
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
                    this.fromFile( read );
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

            streamReader.Close();
        }

        /// <summary>
        /// Nacita vlastnosti environmentu zo stringu, ktory zodpoveda textovemu zapisu World-u.
        /// </summary>
        private void fromFile( string info )
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                info = info.Replace( ".", "," );

            string[] s = info.Split( ' ' );

            Gravity.x = float.Parse( s[ 1 ] );
            Gravity.y = float.Parse( s[ 2 ] );
            AirFriction = float.Parse( s[ 3 ] );
            Collisions = bool.Parse( s[ 4 ] );
            Walls = bool.Parse( s[ 5 ] );
            MaxID = long.Parse( s[ 6 ] );
        }

        public override string ToString()
        {
            string write = "ENV " +
                           Gravity.x.ToString() + " " + Gravity.y.ToString() + " " +
                           AirFriction.ToString() + " " +
                           Collisions.ToString() + " " +
                           Walls.ToString() + " " +
                           MaxID.ToString();
            write = write.Replace( ',', '.' );
            return write;
        }
    }
}
