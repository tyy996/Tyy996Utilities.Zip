
namespace Tyy996Utilities.Zip
{
    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point point, int value)
        {
            return new Point(point.X + value, point.Y + value);
        }

        public static Point operator +(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point operator -(Point point, int value)
        {
            return new Point(point.X - value, point.Y - value);
        }

        public static Point operator -(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static Point operator *(Point point, int value)
        {
            return new Point(point.X * value, point.Y * value);
        }

        public static Point operator *(Point point1, Point point2)
        {
            return new Point(point1.X * point2.X, point1.Y * point2.Y);
        }

        public static Point operator /(Point point, int value)
        {
            return new Point(point.X / value, point.Y / value);
        }

        public static Point operator /(Point point1, Point point2)
        {
            return new Point(point1.X / point2.X, point1.Y / point2.Y);
        }

        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            return this == (Point)obj;    
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return string.Format("(X,Y): {0},{1}", X, Y);
        }
    }
}
