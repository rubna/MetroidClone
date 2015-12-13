using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid;
using MetroidClone.Metroid.Monsters;
using MetroidClone.Engine.Solids;

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
        public Random Random;

        public float Width { get; protected set; } = 8000;
        public float Height { get; protected set; } = 8000;

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
        }

        public void Initialize()
        {
            (new LevelGenerator()).Generate(this);
            Player = new Player();
            AddObject(Player, 80, 80);
            AddObject(new TestMonster(), 100, 50);

            // foreach (GameObject gameObject in GameObjects)
            //gameObject.Create();

            shouldUpdateSolidGrid = true;
        }

        public void UpdateSolidGrid()
        {
            int hCells = (int) (Width / GridSize), vCells = (int) (Height / GridSize);
            SolidGrid = new List<ISolid>[hCells, vCells];
            List<ISolid> solids = Solids;
            for (int i = 0; i < hCells; i++)
            {
                for (int j = 0; j < vCells; j++)
                {
                    Rectangle boundingbox = new Rectangle((int) ((i - 1) * GridSize), (int) ((j - 1) * GridSize), (int) GridSize * 2, (int) GridSize * 2);
                    int numberOfSolids = solids.Count;
                    SolidGrid[i, j] = new List<ISolid>();
                    for (int k = 0; k < numberOfSolids; k++)
                    {
                        if (solids[k].CollidesWith(boundingbox))
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
        }

        public void Draw()
        {
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.Draw();
            DrawWrapper.EndOfDraw();
        }

        public List<ISolid> GetNearSolids(Vector2 position)
        {
            return SolidGrid[(int) (position.X / GridSize), (int) (position.Y / GridSize)];
        }
    }
}
