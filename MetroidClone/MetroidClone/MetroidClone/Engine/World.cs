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
        }

        public void Initialize()
        {
            Level = new Level();
            AddObject(Level);
            Player = new Player();
            AddObject(new JumpThrough(new Rectangle(200, 150, 16, 1)));
            AddObject(Player, 100, 100);
            AddObject(new Drone(), Player.Position);
            AddObject(new TestMonster(), 200, 50);
            AddObject(new GunPickup(), 300, 50);
            AddObject(new RocketPickup(), 250, 50);
            AddObject(new WrenchPickup(), 350, 50);
            //AddObject(new GunLock(), 350, 250);
            AddObject(new SlopeRight(new Rectangle(280, 192, 56,32)), 350, 250);
            AddObject(new SlopeLeft(new Rectangle(220, 192, 56, 32)), 350, 250);

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
