using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid;

namespace MetroidClone.Engine
{
    class World
    {
        public List<GameObject> GameObjects;
        List<GameObject> newGameObjects;

        public DrawWrapper DrawWrapper { get; set; }
        public AssetManager AssetManager { get; set; }
        public Level Level;
        public Player Player;
        public static Random Random;
        public Vector2 Camera;

        public float Width { get; protected set; } = 8000;
        public float Height { get; protected set; } = 8000;

        public float TileWidth => 48;
        public float TileHeight => 48;

        const float GridSize = 100f;
        public List<ISolid>[,] SolidGrid;

        bool shouldUpdateSolidGrid; //Whether we should update the solid grid during the next Update.

        public List<ISolid> Solids
        {
            get
            {
                return GameObjects.OfType<ISolid>().ToList();
            }
        }

        public World()
        {
            GameObjects = new List<GameObject>();
            newGameObjects = new List<GameObject>();

            shouldUpdateSolidGrid = false;

            Random = new Random();

            Camera = new Vector2(0);
        }

        public void Initialize()
        {
            (new WorldGenerator()).Generate(this);
            UpdateCamera(true);

            shouldUpdateSolidGrid = true;
        }

        public void UpdateSolidGrid()
        {
            int hCells = (int)(Width / GridSize), vCells = (int)(Height / GridSize);
            SolidGrid = new List<ISolid>[hCells, vCells];
            List<ISolid> solids = Solids;
            for (int i = 0; i < hCells; i++)
            {
                for (int j = 0; j < vCells; j++)
                {
                    //Get a bounding box with some extra padding around it.
                    Rectangle boundingbox = new Rectangle((int)((i - 1) * GridSize), (int)((j - 1) * GridSize), (int)GridSize * 3, (int)GridSize * 3);
                    int numberOfSolids = solids.Count;
                    SolidGrid[i, j] = new List<ISolid>();
                    for (int k = 0; k < numberOfSolids; k++)
                    {
                        if (!(solids[k] is Wall) || solids[k].CollidesWith(boundingbox))
                        {
                            SolidGrid[i, j].Add(solids[k]);
        }
                    }
                }
            }
        }

        public void AddObject(GameObject gameObject)
        {
            AddObject(gameObject, Vector2.Zero);
        }

        public void AddObject(GameObject gameObject, float x, float y)
        {
            AddObject(gameObject, new Vector2(x, y));
        }

        public void AddObject(GameObject gameObject, Vector2 position)
        {
            gameObject.World = this;
            gameObject.Drawing = DrawWrapper;
            gameObject.Position = position;
            gameObject.Assets = AssetManager;
            newGameObjects.Add(gameObject);
        }

        public void RemoveObject(GameObject gameObject)
        {
            newGameObjects.Remove(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.Update(gameTime);

            List<GameObject> addedObjects = newGameObjects.Select(x => x).ToList();
            addedObjects = addedObjects.Except(GameObjects).ToList();
            foreach (GameObject gameObject in addedObjects)
                gameObject.Create();
            GameObjects = newGameObjects.Select(x => x).ToList();

            if (shouldUpdateSolidGrid)
            {
                shouldUpdateSolidGrid = false;
                UpdateSolidGrid();
            }

            UpdateCamera(); //Update the position of the camera.
        }

        void UpdateCamera(bool jumpToGoal = false)
        {
            float roomWidth = WorldGenerator.LevelWidth * TileWidth, roomHeight = WorldGenerator.LevelHeight * TileHeight;

            //Calculate the basic goal position.
            Vector2 GoalCamera = new Vector2((int)(Player.Position.X / roomWidth) * roomWidth, (int)(Player.Position.Y / roomHeight) * roomHeight);

            //Briefly show a small part of the next room if you bump into a wall there.
            if (Player.TimeSinceHWallCollision < 2)
            {
                if (Player.LastHCollisionDirection == Direction.Right && Player.Position.X % roomWidth > roomWidth - TileWidth)
                    GoalCamera.X += TileWidth;
                if (Player.LastHCollisionDirection == Direction.Left && Player.Position.X % roomWidth < TileWidth)
                    GoalCamera.X -= TileWidth;
            }
            if (Player.TimeSinceVWallCollision < 2)
            {
                if (Player.LastVCollisionDirection == Direction.Down && Player.Position.Y % roomHeight > roomHeight - TileHeight)
                    GoalCamera.Y += TileHeight;
            }
            if (Player.TimeSinceVWallCollision < 12)
            {
                if (Player.LastVCollisionDirection == Direction.Up && Player.Position.Y % roomHeight < TileHeight)
                    GoalCamera.Y -= TileHeight;
            }

            if (jumpToGoal)
                Camera = GoalCamera;
            else
            {
                //Move the camera towards the goal if the goal camera position if not equal to the camera position.
                if (Camera.X < GoalCamera.X)
                    Camera.X = Math.Min(GoalCamera.X, Camera.X + (GoalCamera.X - Camera.X) * 0.12f + 3f);
                if (Camera.X > GoalCamera.X)
                    Camera.X = Math.Max(GoalCamera.X, Camera.X + (GoalCamera.X - Camera.X) * 0.12f - 3f);
                if (Camera.Y < GoalCamera.Y)
                    Camera.Y = Math.Min(GoalCamera.Y, Camera.Y + (GoalCamera.Y - Camera.Y) * 0.12f + 3f);
                if (Camera.Y > GoalCamera.Y)
                    Camera.Y = Math.Max(GoalCamera.Y, Camera.Y + (GoalCamera.Y - Camera.Y) * 0.12f - 3f);
            }
        }

        public void Draw()
        {
            //TODO: Only draw objects that are visible.
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.Draw();
        }

        public void DrawGUI()
        {
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.DrawGUI();
        }

        public List<ISolid> GetNearSolids(Vector2 position)
        {
            if (position.X > -GridSize && position.Y > -GridSize && position.X < Width && position.Y < Height)
                return SolidGrid[(int)(position.X / GridSize), (int)(position.Y / GridSize)];
            else
                return new List<ISolid>(); //Nothing here.
        }

    }
}
