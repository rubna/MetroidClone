using Microsoft.Xna.Framework;

namespace FloppyWieners
{
     class GameObject
    {
        public Vector2 Position = Vector2.Zero;
        public Vector2 Speed = Vector2.Zero;
        public World World;
        public DrawWrapper Drawing;
        public float Friction = 0.9f;
        public InputHelper Input;
        public float Gravity = 0;

        public virtual void Create()
        {
            Input = InputHelper.Instance;
        }

        public virtual void Update()
        {
            Speed.Y += Gravity;
            Speed *= Friction;
            Position += Speed;
        }

        public virtual void Draw()
        {
        }
    }
}