using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;
using MetroidClone.Engine;
using System.Timers;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerRocket : PhysicsObject, IPlayerAttack
    {
        public float Damage => 3;

        private Vector2 direction;
        int smokeTrailTime;

        public override void Create()
        {
            base.Create();
            smokeTrailTime = 0;
            Gravity = 0f;
            Friction.X = 0.99f;
            BoundingBox = new Rectangle(-4, -2, 8, 4);
            SetSprite("Robot/Rocket");
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

            Speed = direction * 2;
        }

        public override void Update(GameTime gameTime)
        {
            ImageRotation = MathHelper.ToRadians(VectorExtensions.Angle(Speed));
            if (smokeTrailTime > 5)
            {
                //World.AddObject(new Smoke(), Position);
                smokeTrailTime = 0;
            }
            else
                smokeTrailTime++;
            Speed += direction * 0.25f;
            base.Update(gameTime);

            if (HadHCollision || HadVCollision)  
                Destroy();

            if (World.PointOutOfView(Position, -10))
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(DrawBoundingBox, Color.Green);
            //base.Draw();
            Drawing.DrawSprite(CurrentSprite, DrawPosition, 0, CurrentSprite.Size * new Vector2(GetFlip, 1) * 0.3f, null, ImageRotation);// ImageScaling, Color.White, ImageRotation);
        }

        public override void Destroy()
        {
            base.Destroy();
            //Audio.Play("Audio/Combat/Gunshots/Rocket/Explosion");
            if (!World.PointOutOfView(Position, -10))
                World.AddObject(new PlayerExplosion(), Position);
        }
    }
}
