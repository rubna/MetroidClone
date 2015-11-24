using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MetroidClone.Engine
{
    class GameObject
    {
        public Vector2 Position = Vector2.Zero;
        public bool FlipX = false;
        public int GetFlip
        {
            get
            {
                if (FlipX)
                    return -1;
                else
                    return 1;
            }
        }

        public float Depth = 0;

        public World World;
        public DrawWrapper Drawing;
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
