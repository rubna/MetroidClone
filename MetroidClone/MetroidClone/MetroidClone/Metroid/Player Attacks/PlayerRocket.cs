using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;
using MetroidClone.Engine;
using System.Timers;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerRocket : PhysicsObject, IPlayerAttack
    {
        private Vector2 direction;
        int smokeTrailTime;

        public override void Create()
        {
            base.Create();
            smokeTrailTime = 0;
            Gravity = 0f;
            Friction.X = 0.99f;
            BoundingBox = new Rectangle(-4, -2, 8, 4);

            //different methods set direction depending on the controls used
            if (Input.ControllerInUse)
            {
                direction = Input.ThumbStickCheckDirection(false);
                direction.Y = -direction.Y;
            }
            else
            {
                direction = Input.MouseCheckUnscaledPosition(Drawing).ToVector2() - DrawPosition;
            }
            direction.Normalize();

            Speed = direction;
        }

        public override void Update(GameTime gameTime)
        {
            if (smokeTrailTime > 5)
            {
                World.AddObject(new Smoke(), Position);
                smokeTrailTime = 0;
            }
            else
                smokeTrailTime++;
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
            Audio.Play("Audio/Combat/Gunshots/Rocket/Explosion");
            World.AddObject(new PlayerExplosion(), Position);
        }
    }
}
