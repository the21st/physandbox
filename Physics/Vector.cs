using System;

namespace Physics
{
    public class Vector
    {
        public float x, y;
        public byte Tag = 0;

        public Vector()
        {
            x = y = 0;
        }

        public Vector( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public Vector( float x, float y )
        {
            this.x = x;
            this.y = y;
        }

        public Vector( Vector v )
        {
            this.x = v.x;
            this.y = v.y;
        }

        public float Abs()
        {
            return (float)Math.Sqrt( x * x + y * y );
        }

        public Vector Normalized()
        {
            return this / this.Abs();
        }

        public Vector Perpendicular()
        {
            if (this.x == 0)
            {
                float y2 = -this.x / this.y;
                return new Vector( 1, y2 );
            }

            float x2 = -this.y / this.x;
            return new Vector( x2, 1 );
        }

        public static Vector Projection( Vector v, Vector target )
        {
            return ((v * target) / (target.Abs() * target.Abs())) * target;
        }

        public static float ProjectionLength( Vector v, Vector target )
        {
            return (v * target) / target.Abs();
        }

        public static bool AreParallel( Vector v1, Vector v2 )
        {
            return ((v1.x * v2.y) == (v1.y * v2.x));
        }

        public static bool AreEqual( Vector v1, Vector v2 )
        {
            return ((v1.x == v2.x) && (v1.y == v2.y));
        }

        public static Vector operator +( Vector v1, Vector v2 )
        {
            return new Vector( v1.x + v2.x, v1.y + v2.y );
        }

        public static Vector operator -( Vector v1, Vector v2 )
        {
            return new Vector( v1.x - v2.x, v1.y - v2.y );
        }

        public static Vector operator *( float a, Vector v )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static Vector operator *( Vector v, float a )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static float operator *( Vector v1, Vector v2 )
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static Vector operator /( Vector v, float a )
        {
            return new Vector( v.x / a, v.y / a );
        }

        public override string ToString()
        {
            return this.x + ", " + this.y;
        }
    }
}
