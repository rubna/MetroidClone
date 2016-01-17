using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;
using MetroidClone.Engine;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerRocket : PhysicsObject, IPlayerAttack
    {
        private Vector2 direction;

        public override void Create()
        {
            base.Create();
            Gravity = 0f;
            Friction.X = 0.99f;
            BoundingBox = new Rectangle(-4, -2, 8, 4);

            //different methods set direction depending on the controls used
            if (Input.ControllerInUse)
            {
                direction = Input.ThumbStickCheckDirection(false);
                direction.Y = -direction.Y;
;
            }
            else
            {
                direction = Input.MouseCheckUnscaledPosition(Drawing).ToVector2() - Position;
            }
            direction.Normalize();

            Speed = direction;
        }

        public override void Update(GameTime gameTime)
        {
            Speed += direction * 0.2f;
            base.Update(gameTime);
            if (HadHCollision || HadVCollision)
                Destroy();

            if (World.PointOutOfView(Position, -10))
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(DrawBoundingBox, Color.Green);
            base.Draw();
        }

        public override void Destroy()
        {
            base.Destroy();
            World.AddObject(new PlayerExplosion(), Position);
        }
    }
}
