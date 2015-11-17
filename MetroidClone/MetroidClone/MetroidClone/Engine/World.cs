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
        
        public DrawWrapper DrawWrapper { get; set; }
        public Level Level;
        public Player Player;

        public World()
        {
            GameObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            Level = new Level();
            AddObject(Level);
            Player = new Player();
            Player.Position = new Vector2(50, 50);
            AddObject(Player);

            foreach (GameObject gameObject in GameObjects)
                gameObject.Create();
        }

        public void AddObject(GameObject gameObject)
        {
            gameObject.World = this;
            gameObject.Drawing = DrawWrapper;
            GameObjects.Add(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.Update(gameTime);
        }

        public void Draw()
        {
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.Draw();
            DrawWrapper.EndOfDraw();
        }


    }
}
