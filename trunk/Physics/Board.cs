using System;
using System.Drawing;

namespace Physics
{
    /// <summary>
    /// Trieda, ktora reprezentuje dosku (odrazovu usecku) ako fyzikalny objekt.
    /// Doska je definovana ako usecka
    /// </summary>
    public class Board : PhysicsObject
    {
        public Line line; // usecka definujuca dosku

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

        /// <summary>
        /// Vykresli dosku na platno World-u, ktoremu doska prislucha.
        /// </summary>
        public override void Render()
        {
            Graphics g = world.Graph;
            Color selectedClr = Clr;
            Pen p = new Pen( Clr, 2 );

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
                p.Width = 3;
            }

            g.DrawLine( p, line.Start.x, line.Start.y, line.End.x, line.End.y );

            p.Dispose();
        }

        /// <summary>
        /// Zisti, ci sa doska nachadza niekde v okoli danych suradnic.
        /// </summary>
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

        /// <summary>
        /// Nacita vlastnosti dosky zo stringu, ktory zodpoveda jej textovemu zapisu.
        /// </summary>
        public void FromFile( string info )
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                info = info.Replace( ".", "," );

            string[] s = info.Split( ' ' );

            line.Start.x = float.Parse( s[ 1 ] );
            line.Start.y = float.Parse( s[ 2 ] );
            line.End.x = float.Parse( s[ 3 ] );
            line.End.y = float.Parse( s[ 4 ] );
            Clr = Color.FromArgb( int.Parse( s[ 5 ] ), int.Parse( s[ 6 ] ), int.Parse( s[ 7 ] ) );
            ID = long.Parse( s[ 8 ] );
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
    }
}
