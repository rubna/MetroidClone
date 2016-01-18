using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class AStarMap
    {
        public enum NavPoint
        {
            none, platform, leftEdge, rightEdge, solo
        }

        public bool[,] WorldGrid;

        public AStarMap(bool[,] worldGrid)
        {
            WorldGrid = worldGrid;
        }

        public AStarPoint[,] NavMesh()
        {
            AStarPoint[,] navMesh = new AStarPoint[WorldGenerator.LevelWidth * WorldGenerator.WorldWidth,
                WorldGenerator.LevelHeight * WorldGenerator.WorldHeight];
            bool platformStarted = false;
            for (int y = 0; y < WorldGenerator.LevelHeight * WorldGenerator.WorldHeight; y++)
            {
                platformStarted = false;
                for (int x = 0; x < WorldGenerator.LevelWidth * WorldGenerator.WorldWidth; x++)
                {
                    if (!platformStarted)
                    {
                        if (!WorldGrid[x, y] && WorldGrid[x, y + 1])
                        {

                            navMesh[x, y] = new AStarPoint(x,y,(int)NavPoint.leftEdge);
                            platformStarted = true;
                            if (WorldGrid[x + 1, y] || !WorldGrid[x + 1, y + 1])
                            {
                                navMesh[x, y] = new AStarPoint(x, y, (int)NavPoint.solo);
                                platformStarted = false;
                            }
                        }
                        else
                            navMesh[x, y] = new AStarPoint(x, y, (int)NavPoint.none);
                    }
                    if (platformStarted)

                        if (!WorldGrid[x, y] && WorldGrid[x, y + 1])
                        {
                            navMesh[x, y] = new AStarPoint(x, y, (int)NavPoint.platform);
                            if (WorldGrid[x + 1, y] || WorldGrid[x + 1, y + 1])
                                navMesh[x, y] = new AStarPoint(x, y, (int)NavPoint.rightEdge);
                        }
                }
            }
            return navMesh;
        }
    }
}
