using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class AStarPoint
    {
        public Vector2 Coordinates;
        public int NavPointType;

        public AStarPoint(int x, int y, int pointType)
        {
            Coordinates = new Vector2(x*WorldGenerator.WorldWidth, y*WorldGenerator.WorldWidth);
            NavPointType = pointType;
        }
    }
}
