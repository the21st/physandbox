using System;
using System.Drawing;

namespace Physics
{
    public class Board : PhysicsObject
    {
        public Line line;

        public Board( World world )
        {
            this.world = world;
            ID = ++world.MaxID;

            line = new Line();
            Clr = Color.Black;
        }

        public override void Tick( float time )
        {
            // asi nic
        }

        public override void Render()
        {
            Graphics g = world.Graph;
            Pen p = new Pen( Clr );

            if (selected)
            {
                p.Width = 2;

                if (phase >= maxChange)
                    brighter = false;
                if (phase <= -maxChange)
                    brighter = true;

                int red = Clr.R;
                int green = Clr.G;
                int blue = Clr.B;

                if (brighter)
                {
                    red = red + 3 > 255 ? 255 : red + 3;
                    green = green + 3 > 255 ? 255 : green + 3;
                    blue = blue + 3 > 255 ? 255 : blue + 3;
                    Clr = Color.FromArgb( red, green, blue );
                    phase++;
                }
                else
                {
                    red = red - 3 < 0 ? 0 : red - 3;
                    green = green - 3 < 0 ? 0 : green - 3;
                    blue = blue - 3 < 0 ? 0 : blue - 3;
                    Clr = Color.FromArgb( red, green, blue );
                    phase--;
                }
            }

            //p.Width = 1; //%%%

            g.DrawLine( p, line.Start.x, line.Start.y, line.End.x, line.End.y );

            p.Dispose();
        }

        public override string ToString()
        {
            string write = "BOA " +
                           line.Start.x.ToString() + " " + line.Start.y.ToString() + " " +
                           line.End.x.ToString() + " " + line.End.y.ToString() + " " +
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

            line.Start.x = float.Parse( s[ 1 ] );
            line.Start.y =  float.Parse( s[ 2 ] );
            line.End.x = float.Parse( s[ 3 ] );
            line.End.y = float.Parse( s[ 4 ] );
            Clr = Color.FromArgb( int.Parse( s[ 5 ] ), int.Parse( s[ 6 ] ), int.Parse( s[ 7 ] ) );
            ID = long.Parse( s[ 8 ] );
        }

        public bool IsAtLocation( float x, float y )
        {
            Sphere tempSphere = new Sphere( null );
            tempSphere.Location.x = x;
            tempSphere.Location.y = y;
            tempSphere.Radius = 3;

            if (Geometry.Intersects( tempSphere, this.line ))
                return true;
            return false;
        }
    }
}
