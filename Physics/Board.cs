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
    }
}
