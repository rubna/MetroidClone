using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FloppyWieners
{
    class World
    {
        public List<GameObject> GameObjects;
        List<GameObject> nextGameObjects;
        public DrawWrapper Drawing;

        public void Create()
        {
            GameObjects = new List<GameObject>();
            nextGameObjects = new List<GameObject>();
            AddObject(new Piemel() { Position = new Vector2(200, 200) });
            
            RecurseCreate();
        }

        void RecurseCreate()
        {
            List<GameObject> addedObjects = nextGameObjects.Select(x => x).ToList();
            addedObjects = addedObjects.Except(GameObjects).ToList();
            GameObjects = nextGameObjects.Select(x => x).ToList();
            foreach (GameObject gameObject in addedObjects)
                gameObject.Create();
            if (nextGameObjects.Except(GameObjects).Count() > 0)
                RecurseCreate();
        }

        public void Update()
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.Update();


            InputHelper.Instance.Update();
            //update game object list
            List<GameObject> addedObjects = nextGameObjects.Select(x => x).ToList();
            addedObjects = addedObjects.Except(GameObjects).ToList();
            foreach (GameObject gameObject in addedObjects)
                gameObject.Create();
            GameObjects = nextGameObjects.Select(x => x).ToList();
        }

        public void Draw()
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.Draw();
        }

        public void AddObject(GameObject gameObject)
        {
            gameObject.World = this;
            gameObject.Drawing = Drawing;
            nextGameObjects.Add(gameObject);
        }
    }
}
