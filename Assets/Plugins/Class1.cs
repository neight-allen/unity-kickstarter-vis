using System;
using System.Collections.Generic;
using System.Text;

namespace Delaunay_triangulation
{
    public class DelaunayTriangulation
    {
        List<Point2D> points;
        List<Point2D> leftOverPoints=new List<Point2D>();
        internal List<Triangule> res = new List<Triangule>();
        public DelaunayTriangulation(List<Point2D>points)
        {
            this.points = points;
            foreach (Point2D p in points)
                leftOverPoints.Add((Point2D)p.Clone());
        }

        public List<Triangule> Triangulate(List<Triangule> triangules, Point2D newPoint)
        {
            List<Point2D> l = new List<Point2D>();
            l.Add(newPoint);
            return Triangulate(l, triangules).res;
        }

        public List<Triangule> Triangulate()
        {
            res.Clear();
            Triangule root = new Triangule();
            Pair pair;
            List<Point2D> leftOverPointsTemp = new List<Point2D>(leftOverPoints);
            List<Point2D> leftOverPoints2 = new List<Point2D>(leftOverPoints);
            foreach (Point2D[] p in GetRandomPoints())
            {
                if (!HasPointsInside(p[0], p[1], p[2]))
                {
                    root = new Triangule(p[0], p[1], p[2]);
                    res.Add(root);
                    leftOverPointsTemp.Remove(p[0]);
                    leftOverPointsTemp.Remove(p[1]);
                    leftOverPointsTemp.Remove(p[2]);
                    pair = Triangulate(leftOverPointsTemp, res);
                    if (pair.ok)
                        return pair.res;
                    else
                    {
                        res.RemoveAt(0);
                        leftOverPointsTemp = leftOverPoints2;
                    }
                }
            }
            return null;
        }

        private Pair Triangulate(List<Point2D>leftOverPoints,List<Triangule>res)
        {
            List<Triangule> temp=null;
            Point2D newPoint = new Point2D();
            while (leftOverPoints.Count > 0)
            {
                for (int i = 0; i < leftOverPoints.Count; i++)
                {
                    temp = GetNextTriangules(leftOverPoints[i],res);
                    if (temp.Count > 0)
                    {
                        res.AddRange(temp);
                        newPoint=leftOverPoints[i];
                        leftOverPoints.RemoveAt(i);
                        int countTriangules = res.Count;
                        Pair p=Triangulate(leftOverPoints, res);
                        if (p.ok)
                            return p;
                        else
                        {
                            res.RemoveRange(res.Count - temp.Count, temp.Count);
                            leftOverPoints.Insert(i, newPoint);
                        }
                    }
                }
                return new Pair(res,false);
            }
            return new Pair(res,true);
        }

        private List<Triangule> GetNextTriangules(Point2D newPoint,List<Triangule>res)
        {
            List<Triangule> result = new List<Triangule>();
            foreach (Point2D[] p in GetPairRandomPoints())
            {
                if (!p[0].Equals(newPoint) && !p[1].Equals(newPoint))
                {
                    if (!HasPointsInside(p[0],p[1],newPoint))
                    { result.Add(new Triangule(p[0],p[1],newPoint)); }
                }                
            }
            return result;
        }

        Triangule TakeValidTriangule()
        {
            foreach(Point2D[] p in GetRandomPoints())
            {
                if (!HasPointsInside(p[0], p[1], p[2]))
                    return new Triangule(p[0],p[1],p[2]);
            }
            return null;
        }

        private bool HasPointsInside(Point2D point2D, Point2D point2D_2, Point2D point2D_3)
        {
            List<Triangule> list = new List<Triangule>();
            Triangule t = new Triangule(point2D, point2D_2, point2D_3);
            list.Add(t);
            Dictionary<Triangule, Circle> dict = GetCircumcicles(list);
            foreach (Point2D p in points)
            {
                if (!p.Equals(point2D) && !p.Equals(point2D_2) && !p.Equals(point2D_3))
                {
                    if (Math.Pow(p.x - dict[t].center.x, 2) + Math.Pow(p.y - dict[t].center.y, 2) <= Math.Pow(dict[t].radius, 2))
                        return true;
                }
            }
            return false;
            //float[,] matrix = new float[3, 3];
            //Triangule t = new Triangule(point2D, point2D_2, point2D_3);
            //foreach (Point2D p in points)
            //{
            //    //if (res.Find(t => t.a.Equals(p) || t.b.Equals(p) || t.c.Equals(p)) == null && !p.Equals(point2D) && !p.Equals(point2D_2) && !p.Equals(point2D_3))
            //    if (!p.Equals(point2D) && !p.Equals(point2D_2) && !p.Equals(point2D_3))
            //    {
            //        matrix = new float[3, 3];
            //        matrix[0, 0] = t.a.x - p.x;
            //        matrix[0, 1] = t.a.y - p.y;
            //        matrix[0, 2] = (float)(Math.Pow(matrix[0, 0], 2) + Math.Pow(matrix[0, 1], 2));
            //        matrix[1, 0] = t.b.x - p.x;
            //        matrix[1, 1] = t.b.y - p.y;
            //        matrix[1, 2] = (float)(Math.Pow(matrix[1, 0], 2) + Math.Pow(matrix[1, 1], 2));
            //        matrix[2, 0] = t.c.x - p.x;
            //        matrix[2, 1] = t.c.y - p.y;
            //        matrix[2, 2] = (float)(Math.Pow(matrix[2, 0], 2) + Math.Pow(matrix[2, 1], 2));

            //        if (CalculateDeterminant(matrix) > 0)
            //            return true;
            //    }
            //}
            //return false;
        }

        private float CalculateDeterminant(float[,] matrix)
        {
            float res = 0;
            res = (matrix[0, 0] * matrix[1, 1] * matrix[2, 2]) + (matrix[1, 0] * matrix[0, 1] * matrix[2, 2])
                + (matrix[0, 0] * matrix[2, 1] * matrix[1, 2]);
            res -= (matrix[2, 0] * matrix[1, 1] * matrix[0, 2]);
            res -= (matrix[0, 1] * matrix[1, 2] * matrix[2, 0]);
            res-= (matrix[0, 2] * matrix[1, 0] * matrix[2, 1]);
            return res;
        }

        IEnumerable<Point2D[]> GetRandomPoints()
        {
            for (int i = 0; i < points.Count; i++)
                for (int j = i + 1; j < points.Count; j++)
                    for (int k = j + 1; k < points.Count; k++)
                        yield return new Point2D[3] { points[i],points[j],points[k]};
        }

        IEnumerable<Point2D[]> GetPairRandomPoints()
        {
            for (int i = 0; i < points.Count; i++)
                for (int j = i + 1; j < points.Count; j++)
                        yield return new Point2D[2] { points[i], points[j]};
        }


        public Dictionary<Triangule,Circle> GetCircumcicles(List<Triangule> triangules)
        {
            Dictionary<Triangule,Circle>res=new Dictionary<Triangule,Circle>();
            Rect r1, r2,r3,r4;
            Point2D intersection;
            float radius;
            if(triangules != null)
                foreach (Triangule t in triangules)
                {
                    r1=GetRectEcuation(t.a, t.b);
                    r2 = GetMediatrixEcuation(t.a, t.b,r1);
                    r3 = GetRectEcuation(t.b,t.c);
                    r4 = GetMediatrixEcuation(t.b,t.c,r3);
                    intersection = GetIntersectPoint(r2, r4);
                    radius=(float)GetDistance(t.a, intersection);
                    res[t] = new Circle(radius, (Point2D)intersection.Clone());
                }
            return res;
        }

        public float GetDistance(Point2D p1, Point2D p2)//con la norma euclideana
        {
            return (float)Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        public Point2D GetIntersectPoint(Rect r1, Rect r2)
        {
            float x = (r2.N - r1.N) / (float)(r1.PendientEval() - r2.PendientEval());
            float y = r1.Eval(x);
            return new Point2D(x,y);
        }

        public Rect GetMediatrixEcuation(Point2D p1, Point2D p2,Rect r)
        {
            Point2D halfPoint = new Point2D((p1.x+p2.x)/2f,(p1.y+p2.y)/2f);
            Rational m = new Rational(r.M.denominator,r.M.numerator * -1);
            return new Rect(m, -1 * m.Eval() * halfPoint.x + halfPoint.y);
        }
        
        public Rect GetRectEcuation(Point2D p1, Point2D p2)
        {
            return new Rect(new Rational((int)(p1.y - p2.y), (int)(p1.x - p2.x)), -1 * p1.x * ((float)(p1.y - p2.y) / (float)(p1.x - p2.x)) + p1.y);
        }
    }


    public class Rect
    {
        public Rational M;
        public float N;
        public Rect() { }
        public Rect(Rational M, float N)
        {
            this.M = M; this.N = N;
        }

        public float PendientEval()
        {
            return (float)M.numerator / (float)M.denominator;
        }

        public float Eval(float x)
        {
            return x * PendientEval() + N;
        }
        
    }

    public class Rational
    {
        public int numerator, denominator;
        public Rational(int numerator, int denominator)
        {
            this.numerator = numerator; this.denominator = denominator;
        }

        public float Eval()
        {
            if (denominator == 0) return 0;
            return (float)numerator / (float)denominator;
        }
    }

    public class Circle : ICloneable
    {
        public float radius = 0;
        public Point2D center=new Point2D();

        public Circle() { }
        public Circle(float radius, Point2D center)
        {
            this.radius = radius;
            this.center = center;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Circle(radius, (Point2D)center.Clone());
        }

        #endregion
    }

    public struct Point2D:ICloneable
    {
        public float x, y;
        public Point2D(float x, float y)
        {
            this.x = x; this.y = y;
        }

        public override bool Equals(object obj)
        {
            Point2D other = (Point2D)obj;
            return other.x == x && other.y == y;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Point2D(x, y);
        }

        #endregion
    }


    public class Triangule:ICloneable
    {
        public Point2D a, b, c;

        bool FindAssign(Point2D a1,Point2D a2,Point2D a3,float low)
        {
            if (a1.x == low)
            {
                if (a2.x == low)
                {
                    if (a1.y > a2.y)
                    {
                        this.a = a1;
                        this.b = a2;
                        this.c = a3;
                        return true;
                    }
                    else { this.a = a2; this.b = a1; this.c = a3; return true; }
                }
                else if (a3.x == low)
                {
                    if (a1.y > a3.y)
                    {
                        this.a = a1;
                        this.b = a3;
                        this.c = a2;
                        return true;
                    }
                    else { this.a = a3; this.b = a1; this.c = a2; return true; }
                }
                else if (a2.x > a3.x)
                { this.a = a3; this.b = a1; this.c = a2; return true; }
                else { this.a = a2; this.b = a1; this.c = a3; }
            }
            return false;
        }
        
        public Triangule(Point2D a, Point2D b, Point2D c)
        {
            if (a.Equals(b) || a.Equals(c) || b.Equals(c))
                throw new Exception("no pueden haber 2 puntos de un triangulo iguales");
            float low = Math.Min(a.x, b.x);
            low = Math.Min(low, c.x);
            if (!FindAssign(a, b, c,low) && !FindAssign(b, a, c,low))
                FindAssign(c,a,b,low);
        }

        public Triangule() { }

        public override bool Equals(object obj)
        {
            Triangule other = obj as Triangule;
            return other.a.Equals(a) && other.b.Equals(b) && other.c.Equals(c);
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Triangule((Point2D)a.Clone(), (Point2D)b.Clone(), (Point2D)c.Clone());
        }

        #endregion
    }

    class Pair
    {
        public List<Triangule> res;public bool ok = false;
        public Pair(List<Triangule>res,bool ok)
        { this.res = res; this.ok = ok; }
    }
}
