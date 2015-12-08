using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid;
using MetroidClone.Metroid.Monsters;

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

        public World()
        {
            GameObjects = new List<GameObject>();
            newGameObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            Level = new Level();
            AddObject(Level);
            Player = new Player();
            AddObject(Player, 50, 50);
            AddObject(new TestMonster(), 100, 50);
            AddObject(new PushableBlock(), 120, 100);

            // foreach (GameObject gameObject in GameObjects)
            //gameObject.Create();
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
        }

        public void Draw()
        {
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.Draw();
            DrawWrapper.EndOfDraw();
        }
    }
}
