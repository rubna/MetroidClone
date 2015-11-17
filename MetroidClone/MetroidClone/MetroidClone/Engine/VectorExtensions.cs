using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    static class VectorExtensions
    {
        public static float AngleDifference(float source, float destination)
        {
            double a;
            a = MathHelper.ToRadians(destination) - MathHelper.ToRadians(source);
            a = Math.Atan2(Math.Sin(a), Math.Cos(a));
            return MathHelper.ToDegrees((float)a);
        }

        public static float Angle(this Vector2 vector)
        {
            return MathHelper.ToDegrees((float)Math.Atan2(vector.Y, vector.X));
        }

        public static float LengthDirectionX(float length, float direction)
        {
            return (float)Math.Cos(MathHelper.ToRadians(direction)) * length;
        }
        public static float LengthDirectionY(float length, float direction)
        {
            return (float)Math.Sin(MathHelper.ToRadians(direction)) * length;
        }

        public static Vector2 ToCartesian(float length, float direction)
        {
            float radDir = MathHelper.ToRadians(direction);
            return new Vector2((float)Math.Cos(radDir) * length, (float)Math.Sin(radDir) * length);
        }

        public static Vector2 ToCartesian(this Vector2 vector)
        {
            return ToCartesian(vector.X, vector.Y);
        }

        public static Vector2 ToPolar(this Vector2 vector)
        {
            return new Vector2(vector.Length(), vector.Angle());
        }
        public static Vector2 ToPolar(float x, float y)
        {
            return ToPolar(new Vector2(x, y));
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        public static Point ClampPoint(this Point point, Point min, Point max)
        {
            return new Point((int)MathHelper.Clamp(point.X, min.X, max.X), (int)MathHelper.Clamp(point.Y, min.Y, max.Y));
        }
    }
}
