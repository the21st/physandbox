using System;
using System.Collections.Generic;

namespace Physics
{
    public static class Geometry
    {
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
                if (intersection( line1, line2 ) == null)
                    return false;
                return true;
            }
            return false;
        }

        private static bool Intersects( Sphere sphere, Line line )
        {
            float a, b, c;

            a = (line.End.x - line.Start.x) * (line.End.x - line.Start.x) + (line.End.y - line.Start.y) * (line.End.y - line.Start.y);
            b = 2 * ((line.End.x - line.Start.x) * (line.Start.x - sphere.Location.x) + (line.End.y - line.Start.y) * (line.Start.y - sphere.Location.y));
            c = sphere.Location.x * sphere.Location.x + sphere.Location.y * sphere.Location.y + line.Start.x * line.Start.x + line.Start.y * line.Start.y
                - 2 * (sphere.Location.x * line.Start.x + sphere.Location.y * line.Start.y) - sphere.Radius * sphere.Radius;

            float disc = b * b - 4 * a * c;

            if (disc <= 0)
                return false;
            return true;
        }

        private static bool pointOnLine( Vector point, Line line )
        {
            float minX = Math.Min( line.Start.x, line.End.x );
            float minY = Math.Min( line.Start.y, line.End.y );
            float maxX = Math.Max( line.Start.x, line.End.x );
            float maxY = Math.Max( line.Start.y, line.End.y );
            return ((point.x >= minX) && (point.y >= minY) && (point.x <= maxX) && (point.y <= maxY));
        }

        private static bool areParallel( Line line1, Line line2 )
        {
            if (Vector.AreParallel( line1.End - line1.Start, line2.End - line2.Start ))
                return true;
            else
                return false;
        }

        private static bool onOneLine( Line line1, Line line2 )
        {
            if (areParallel( line1, line2 )
                &&
                Vector.AreParallel( line1.Start - line2.Start, line1.End - line1.Start ))
                return true;
            else
                return false;
        }

        private static Vector lineIntersection( Line line1, Line line2 )
        {
            //podla vzorca na vypocet priesecniku dvoch useciek:
            float f = ((((line2.End.x - line2.Start.x) * (line1.Start.y - line2.Start.y)) - ((line2.End.y - line2.Start.y) * (line1.Start.x - line2.Start.x))) /
                       (((line2.End.y - line2.Start.y) * (line1.End.x - line1.Start.x)) - ((line2.End.x - line2.Start.x) * (line1.End.y - line1.Start.y))));
            Vector result = new Vector( line1.Start.x + f * (line1.End.x - line1.Start.x), line1.Start.y + f * (line1.End.y - line1.Start.y) );

            return result;
        }

        private static Vector intersection( Line line1, Line line2 )
        {
            //podla vzorca na vypocet priesecniku dvoch useciek:
            float f = ((((line2.End.x - line2.Start.x) * (line1.Start.y - line2.Start.y)) - ((line2.End.y - line2.Start.y) * (line1.Start.x - line2.Start.x))) /
                       (((line2.End.y - line2.Start.y) * (line1.End.x - line1.Start.x)) - ((line2.End.x - line2.Start.x) * (line1.End.y - line1.Start.y))));
            Vector result = new Vector( line1.Start.x + f * (line1.End.x - line1.Start.x), line1.Start.y + f * (line1.End.y - line1.Start.y) );

            if (pointOnLine( result, line1 ) && pointOnLine( result, line2 ))
                return result;
            else
                return null;
        }

        //public static Vector Intersection( Line primary, Line secondary )
        //{
        //    if (areParallel( primary, secondary ))
        //    {
        //        // su rovnobezne
        //        if (onOneLine( primary, secondary ))
        //        {
        //            // lezia na jednej priamke
        //            // rozoberam vsetky mozne pripady:

        //            if (pointOnLine( primary.Start, secondary ))
        //            {
        //                // prienik je hned v zaciatku primarnej usecky
        //                return new Vector( primary.Start );
        //            }

        //            if (pointOnLine( secondary.Start, primary ) &&
        //                pointOnLine( secondary.End, primary ))
        //            {
        //                if ((secondary.Start - primary.Start).Abs() < (secondary.End - primary.Start).Abs())
        //                    return new Vector( secondary.Start );
        //                else
        //                    return new Vector( secondary.End );
        //            }

        //            if (pointOnLine( secondary.Start, primary ))
        //            {
        //                return new Vector( secondary.Start );
        //            }

        //            if (pointOnLine( secondary.End, primary ))
        //            {
        //                return new Vector( secondary.End );
        //            }
        //        }
        //        else
        //        {
        //            // nelezia na jednej priamke, a teda nemaju priesecnik
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        // su roznobezne
        //        return intersection( primary, secondary );
        //    }
        //    return null;
        //}

        private static int sgnA( float x )
        {
            if (x < 0)
                return -1;
            return 1;
        }

        private static Vector midPoint( Vector v1, Vector v2 )
        {
            return new Vector( (v1.x + v2.x) / 2, (v1.y + v2.y) / 2 );
        }

        public static List<Vector> Intersection( Sphere sphere, Line line )
        {
            float a, b, c;

            a = (line.End.x - line.Start.x) * (line.End.x - line.Start.x) + (line.End.y - line.Start.y) * (line.End.y - line.Start.y);
            b = 2 * ((line.End.x - line.Start.x) * (line.Start.x - sphere.Location.x) + (line.End.y - line.Start.y) * (line.Start.y - sphere.Location.y));
            c = sphere.Location.x * sphere.Location.x + sphere.Location.y * sphere.Location.y + line.Start.x * line.Start.x + line.Start.y * line.Start.y
                - 2 * (sphere.Location.x * line.Start.x + sphere.Location.y * line.Start.y) - sphere.Radius * sphere.Radius;

            float disc = b * b - 4 * a * c;

            if (disc < 0)
                return null;

            List<Vector> result = new List<Vector>();

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

        public static Vector Collision( Sphere sphere, Board board, float time )
        {
            Vector movement = time * sphere.Velocity;

            Line move = new Line( sphere.Location, sphere.Location + movement );

            Vector tmp = movement.Perpendicular();
            tmp = tmp.Normalized();
            tmp = sphere.Radius * tmp;

            Line moveLeft = new Line( sphere.Location + tmp, sphere.Location + tmp + movement );
            Line moveRight = new Line( sphere.Location - tmp, sphere.Location - tmp + movement );

            if (Intersects( move, board.line ))
            {
                if (areParallel( move, board.line ))
                {
                    float min = int.MaxValue;
                    Vector minV = null;
                    List<Vector> intersectionList;
                    Sphere sphereTmp = new Sphere( null );
                    sphereTmp.Radius = sphere.Radius;


                    sphereTmp.Location = board.line.Start;
                    intersectionList = Intersection( sphereTmp, board.line );

                    foreach (Vector v in intersectionList)
                    {
                        float dist = (v - sphere.Location).Abs();
                        if ((dist < min) && (pointOnLine( v, move )))
                        {
                            min = dist;
                            minV = v;
                        }
                    }

                    sphereTmp.Location = board.line.End;
                    intersectionList = Intersection( sphereTmp, board.line );

                    foreach (Vector v in intersectionList)
                    {
                        float dist = (v - sphere.Location).Abs();
                        if ((dist < min) && (pointOnLine( v, move )))
                        {
                            min = dist;
                            minV = v;
                        }
                    }

                    return minV;
                }
                else
                {
                    Line parallel1 = new Line( board.line );
                    Line parallel2 = new Line( board.line );

                    Vector tmpV = (parallel1.End - parallel1.Start).Perpendicular();
                    tmpV = tmpV.Normalized();
                    tmpV = sphere.Radius * tmpV;

                    parallel1.Start = parallel1.Start + tmpV;
                    parallel1.End = parallel1.End + tmpV;

                    parallel2.Start = parallel2.Start - tmpV;
                    parallel2.End = parallel2.End - tmpV;

                    Vector intersection1 = lineIntersection( move, parallel1 );
                    Vector intersection2 = lineIntersection( move, parallel2 );

                    if ((intersection1 - sphere.Location).Abs() < (intersection2 - sphere.Location).Abs())
                        return intersection1;
                    return intersection2;
                }
            }
            else
            {

            }





            return null;
        }
    }
}
