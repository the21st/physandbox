using System;

namespace Physics
{
    /// <summary>
    /// Tato trieda sluzi na vacsinu vypoctov tykajucich sa pozicii a pohybu v 2 rozmeroch.
    /// </summary>
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

        /// <summary>
        /// Konstruktor, ktory vytvori novy vektor totozny so zadanym vektorom.
        /// </summary>
        public Vector( Vector v )
        {
            this.x = v.x;
            this.y = v.y;
        }

        /// <summary>
        /// Vypocitace euklidovsku vzdialenost suradnic vektora od pociatku (tj. jeho dlzku).
        /// </summary>
        public float Abs()
        {
            return (float)Math.Sqrt( x * x + y * y );
        }

        /// <summary>
        /// Vrati novy vektor, ktory ma rovnaky smer ako vektor povodny, ale jeho dlzka je 1.
        /// </summary>
        public Vector Normalized()
        {
            return this / this.Abs();
        }

        /// <summary>
        /// Vrati novy vektor ktory je kolmy na povodny vektor.
        /// Jeho orientacia a dlzka su arbitrarne.
        /// </summary>
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

        /// <summary>
        /// Vrati novy vektor, ktory je projekciou jedneho vektora na druhy. 
        /// </summary>
        public static Vector Projection( Vector v, Vector target )
        {
            return ((v * target) / (target.Abs() * target.Abs())) * target;
        }

        /// <summary>
        /// Vrati dlzku projekcie, ale berie do uvahy aj orientaciu.
        /// Teda moze vratit aj zapornu hodnotu (ak je projekcia opacne orientovana ako cielovy vektor).
        /// </summary>
        public static float ProjectionLength( Vector v, Vector target )
        {
            return (v * target) / target.Abs();
        }

        /// <summary>
        /// Zisti, ci su dane vektory rovnobezne. 
        /// </summary>
        public static bool AreParallel( Vector v1, Vector v2 )
        {
            return ((v1.x * v2.y) == (v1.y * v2.x));
        }

        /// <summary>
        /// Zisti, ci su dane vektory totozne.
        /// </summary>
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

        // Sucin skalaru a vektoru
        public static Vector operator *( float a, Vector v )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static Vector operator *( Vector v, float a )
        {
            return new Vector( a * v.x, a * v.y );
        }

        public static Vector operator /( Vector v, float a )
        {
            return new Vector( v.x / a, v.y / a );
        }

        // Skalarny sucin
        public static float operator *( Vector v1, Vector v2 )
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public override string ToString()
        {
            return this.x + ", " + this.y;
        }
    }
}
