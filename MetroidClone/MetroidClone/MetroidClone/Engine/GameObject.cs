using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MetroidClone.Engine
{
    class GameObject
    {
        public Vector2 Position = Vector2.Zero;

        public float Depth = 0;

        public World World;
        public DrawWrapper Drawing;
        public AssetManager Assets;
        protected InputHelper Input;

        public virtual void Create()
        {
            Input = InputHelper.Instance;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw()
        {

        }
    }
}
