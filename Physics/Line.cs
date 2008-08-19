using System;

namespace Physics
{
    public class Line
    {
        public Vector Start, End;

        public Line()
        {
            Start = new Vector();
            End = new Vector();
        }

        public Line( Vector start, Vector end )
        {
            Start = start;
            End = end;
        }

        public Line( float x1, float y1, float x2, float y2 )
        {
            Start = new Vector( x1, y1 );
            End = new Vector( x2, y2 );
        }

        public Line( Line copy )
        {
            Start = new Vector( copy.Start.x, copy.Start.y );
            End = new Vector( copy.End.x, copy.End.y );
        }
    }
}
