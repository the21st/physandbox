using System;
using System.Collections.Generic;

namespace Physics
{
    /// <summary>
    /// Staticka trieda, ktora obsahuje rozne geometricke funkcie, tykajuce sa najma priamok, useciek, kruznic a ich priesecnikov.
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// Zistuje ci sa dve usecky pretinaju v aspon jednom bode.
        /// </summary>
        private static bool Intersects( Line line1, Line line2 )
        {
            if (areParallel( line1, line2 ))
            {
                // su rovnobezne
                if (onOneLine( line1, line2 ))
                {
                    // lezia na jednej priamke
                    // rozoberam vsetky mozne pripady:

                    if (pointOnLine( line1.Start, line2 ))
                    {
                        return true;
                    }

                    if (pointOnLine( line2.Start, line1 ) &&
                        pointOnLine( line2.End, line1 ))
                    {
                        return true;
                    }

                    if (pointOnLine( line2.Start, line1 ))
                    {
                        return true;
                    }

                    if (pointOnLine( line2.End, line1 ))
                    {
                        return true;
                    }
                }
                else
                {
                    // nelezia na jednej priamke, a teda nemaju priesecnik
                    return false;
                }
            }
            else
            {
                // su roznobezne
                Vector result = lineIntersection( line1, line2 );
                if (pointOnLine( result, line1 ) && pointOnLine( result, line2 ))
                    return true;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Zistuje, ci sa kruznica (reprezentovana triedou Sphere) prelina s useckou (ak sa jej iba dotyka v jednom bode, vrati false).
        /// </summary>
        public static bool Intersects( Sphere sphere, Line line )
        {
            // nasledujuce pocty su vzorce analytickej geometrie na hladanie priesecniku

            float a, b, c;

            a = (line.End.x - line.Start.x) * (line.End.x - line.Start.x) + (line.End.y - line.Start.y) * (line.End.y - line.Start.y);
            b = 2 * ((line.End.x - line.Start.x) * (line.Start.x - sphere.Location.x) + (line.End.y - line.Start.y) * (line.Start.y - sphere.Location.y));
            c = sphere.Location.x * sphere.Location.x + sphere.Location.y * sphere.Location.y + line.Start.x * line.Start.x + line.Start.y * line.Start.y
                - 2 * (sphere.Location.x * line.Start.x + sphere.Location.y * line.Start.y) - sphere.Radius * sphere.Radius;

            float disc = b * b - 4 * a * c;

            if (disc <= 0)
                return false;

            float u = (float)((-b + Math.Sqrt( disc )) / (2 * a));
            Vector p1 = line.Start + u * (line.End - line.Start);

            if (pointOnLine( p1, line ))
                return true;

            float v = (float)((-b - Math.Sqrt( disc )) / (2 * a));
            Vector p2 = line.Start + v * (line.End - line.Start);

            if (pointOnLine( p2, line ))
                return true;

            return false;
        }

        /// <summary>
        /// Zisti, ci dany bod lezi na danej usecke.
        /// POZOR, funkcia predpoklada ze bod lezi na tej istej priamke, ako dana usecka.
        /// </summary>
        private static bool pointOnLine( Vector point, Line line )
        {
            float minX = Math.Min( line.Start.x, line.End.x );
            float minY = Math.Min( line.Start.y, line.End.y );
            float maxX = Math.Max( line.Start.x, line.End.x );
            float maxY = Math.Max( line.Start.y, line.End.y );
            return ((point.x >= minX) && (point.y >= minY) && (point.x <= maxX) && (point.y <= maxY));
        }

        /// <summary>
        /// Zistuje, ci su dane usecky rovnobezne.
        /// </summary>
        private static bool areParallel( Line line1, Line line2 )
        {
            if (Vector.AreParallel( line1.End - line1.Start, line2.End - line2.Start ))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Zistuje, ci dane usecky lezia na tej istej priamke.
        /// </summary>
        private static bool onOneLine( Line line1, Line line2 )
        {
            if (areParallel( line1, line2 )
                &&
                Vector.AreParallel( line1.Start - line2.Start, line1.End - line1.Start ))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Vrati priesecnik (jeho polohovy vektor) priamok, na ktorych lezia zadane usecky.
        /// </summary>
        private static Vector lineIntersection( Line line1, Line line2 )
        {
            if (areParallel( line1, line2 ))
                return null;

            //podla vzorca na vypocet priesecniku dvoch priamok:

            float f = ((((line2.End.x - line2.Start.x) * (line1.Start.y - line2.Start.y)) - ((line2.End.y - line2.Start.y) * (line1.Start.x - line2.Start.x))) /
                       (((line2.End.y - line2.Start.y) * (line1.End.x - line1.Start.x)) - ((line2.End.x - line2.Start.x) * (line1.End.y - line1.Start.y))));
            Vector result = new Vector( line1.Start.x + f * (line1.End.x - line1.Start.x), line1.Start.y + f * (line1.End.y - line1.Start.y) );

            return result;
        }

        /// <summary>
        /// Vrati zoznam priesecnikov kruznice a usecky (teda moze obsahovat 0, 1 alebo 2 prvky).
        /// Ak je ale usecka dotycnicou kruznice, vrati prazdny zoznam.
        /// </summary>
        public static List<Vector> Intersection( Sphere sphere, Line line )
        {
            float a, b, c;
            List<Vector> result = new List<Vector>();

            a = (line.End.x - line.Start.x) * (line.End.x - line.Start.x) + (line.End.y - line.Start.y) * (line.End.y - line.Start.y);
            b = 2 * ((line.End.x - line.Start.x) * (line.Start.x - sphere.Location.x) + (line.End.y - line.Start.y) * (line.Start.y - sphere.Location.y));
            c = sphere.Location.x * sphere.Location.x + sphere.Location.y * sphere.Location.y + line.Start.x * line.Start.x + line.Start.y * line.Start.y
                - 2 * (sphere.Location.x * line.Start.x + sphere.Location.y * line.Start.y) - sphere.Radius * sphere.Radius;

            float disc = b * b - 4 * a * c;

            if (disc <= 0)
                return result;

            float u = (float)((-b + Math.Sqrt( disc )) / (2 * a));

            Vector p1 = line.Start + u * (line.End - line.Start);
            result.Add( p1 );

            if (disc == 0)
            {
                return result;
            }

            float v = (float)((-b - Math.Sqrt( disc )) / (2 * a));

            Vector p2 = line.Start + v * (line.End - line.Start);
            result.Add( p2 );

            return result;
        }

        /// <summary>
        /// Na zaklade parametru time najde najblizsiu koliziu zadanej gule so zadanou doskou.
        /// Vrati objekt ktory obsahuje informacie o tejto zrazke. Ak ku ziadnej kolizii nema nastat, vrati null.
        /// </summary>
        public static CollisionInfo Collision( Sphere sphere, Board board, float time )
        {
            Vector movement = time * sphere.Velocity; // vektor, ktory reprezentuje zmenu polohy gule za casovy usek, ktory skumame

            Line move = new Line( sphere.Location, sphere.Location + movement ); // priamka, ktora priamo reprezentuje trajektoriu sfery

            Vector tmp = movement.Perpendicular(); // pomocny vektor, kolmy na pohyb gule, ktorym si vyrobim rovnobezky k priamke move
            tmp = tmp.Normalized();
            tmp = sphere.Radius * tmp;

            // tu si vyrobim 2 priamky, ktore su rovnobezne s move, a kazda je posunuta inym smerom, tak, aby reprezentovali pohyb laveho a praveho okraju gule
            // a teda aby spolu opisali drahu pohybu celej gule, nielen jej stredu
            Line moveLeft = new Line( sphere.Location + tmp, sphere.Location + tmp + movement );
            Line moveRight = new Line( sphere.Location - tmp, sphere.Location - tmp + movement );

            Sphere sphereMoved = new Sphere( null ); // sfera, ktora reprezentuje konecnu polohu skumanej gule
            sphereMoved.Location = sphere.Location + movement;
            sphereMoved.Radius = sphere.Radius;


            // a tu uz skumam, ci nastala ktorakolvek z nasledujucich kolizii
            if (Intersects( move, board.line ) ||
                Intersects( moveLeft, board.line ) ||
                Intersects( moveRight, board.line ) ||
                Intersects( sphereMoved, board.line ))
            {
                // ak viem, je jedna z tychto kolizii nastala, tak robim nasledujuce:
                // do intersectionList postupne pridavam vsetky rozne priesecniky, ktore by mohli znamenat koliziu

                List<Vector> intersectionList = new List<Vector>();


                // tato gula sluzi na hladanie kolizie nasej gule s koncovymi bodmi dosky
                Sphere sphereTmp = new Sphere( null );
                sphereTmp.Radius = sphere.Radius;

                sphereTmp.Location = board.line.Start;
                List<Vector> listTmp = Intersection( sphereTmp, move );

                foreach (Vector v in listTmp)
                {
                    v.Tag = 2;
                    intersectionList.Add( v );
                }

                sphereTmp.Location = board.line.End;
                listTmp = Intersection( sphereTmp, move );

                foreach (Vector v in listTmp)
                {
                    v.Tag = 2;
                    intersectionList.Add( v );
                }


                // tieto usecky su podobne ako moveLeft a moveRight rovnobezne s doskou, akurat posunute o polomer sfery
                // sluzia na najdenie kolizie gule s doskou, ak to nie je s jednym s koncovych bodov dosky
                Line parallel1 = new Line( board.line );
                Line parallel2 = new Line( board.line );

                Vector shift = (board.line.End - board.line.Start).Perpendicular();
                shift = shift.Normalized();
                shift = sphere.Radius * shift;

                parallel1.Start = parallel1.Start + shift;
                parallel1.End = parallel1.End + shift;

                parallel2.Start = parallel2.Start - shift;
                parallel2.End = parallel2.End - shift;

                Vector intersection1 = lineIntersection( move, parallel1 );
                Vector intersection2 = lineIntersection( move, parallel2 );

                if (pointOnLine( intersection1 + shift, board.line ) || pointOnLine( intersection1 - shift, board.line ))
                {
                    intersection1.Tag = 1;
                    intersectionList.Add( intersection1 );
                }

                if (pointOnLine( intersection2 + shift, board.line ) || pointOnLine( intersection2 - shift, board.line ))
                {
                    intersection2.Tag = 1;
                    intersectionList.Add( intersection2 );
                }

                // Cize ja som do zoznamu intersectionList pridal vsetky mozne polohy kolizii, ktore su pripustne.
                // Ale kedze z podmienky viem, ze k nejakej kolizii doslo - musela to byt ta, ktora je najblizsie k povodnej polohe sfery.
                // A teda tu, ktora je najblizsie vyberiem, a vratim ju ako vysledok tejto funkcie.

                if (intersectionList.Count > 0)
                {
                    float min = int.MaxValue;
                    Vector minV = null;
                    float dist;

                    foreach (Vector v in intersectionList)
                    {
                        dist = (v - sphere.Location).Abs();
                        if ((dist < min) && (pointOnLine( v, move )))
                        {
                            min = dist;
                            minV = v;
                        }
                    }

                    if (minV == null)
                        return null;

                    CollisionInfo result = new CollisionInfo( minV, board, minV.Tag );

                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Vrati suradnice, kam by mala byt posunuta zadana gula (ak sa prekryva so zadanou doskou) tak, aby sa neprekryvali.
        /// </summary>
        public static Vector Overlap( Sphere sphere, Board board )
        {
            if (Intersects( sphere, board.line ))
            {
                // metoda je podobna funkcii Collision

                List<Vector> intersectionList = Intersection( sphere, board.line );
                if (intersectionList.Count == 2)
                {
                    Line parallel1 = new Line( board.line );
                    Line parallel2 = new Line( board.line );

                    Vector shift = (board.line.End - board.line.Start).Perpendicular();
                    shift = shift.Normalized();

                    // V tejto funkcii ale nevratim presny bod, taky, aby sa gula dotykala dosky v jednom bode,
                    // ale bude posunuty o 0.1 pixelu smerom od dosky.
                    Vector correction = new Vector( shift );
                    correction = 0.1f * correction;
                    shift = sphere.Radius * shift;
                    shift += correction;

                    parallel1.Start = parallel1.Start + shift;
                    parallel1.End = parallel1.End + shift;

                    parallel2.Start = parallel2.Start - shift;
                    parallel2.End = parallel2.End - shift;

                    Vector tempV = board.line.End - board.line.Start;
                    tempV = tempV.Perpendicular();
                    Line tempLine = new Line( sphere.Location, sphere.Location + tempV );

                    Vector intersection1 = lineIntersection( tempLine, parallel1 );
                    Vector intersection2 = lineIntersection( tempLine, parallel2 );

                    if ((intersection1 - sphere.Location).Abs() < (intersection2 - sphere.Location).Abs())
                        return intersection1;
                    else
                        return intersection2;
                }
            }
            return null;
        }
    }
}
