using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class Drone : PhysicsObject
    {
        Monster target = null;
        float setTargetTimer = 0;
        Vector2 targetPosition = Vector2.Zero;
        public DroneState CurrentState = DroneState.Following;
        float targetRot = 0f;
        Vector2 acceleration = Vector2.Zero;
        float maxAcceleration = 1;
        public static int checkDroneCollision = 0;
        float attentionRadius = 300;

        float shootTimer = 0;

        public override void Create()
        {
            base.Create();
            Gravity = 0;
            Friction = Vector2.One * 0.93f;
            BoundingBox = new Rectangle(-8, -8, 16, 16);
            WallBounce = Vector2.One * 0.9f;
            SetSprite("Items/dronesmall");
            IgnoreJumpThroughs = true;
            Depth = 2;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (CurrentState == DroneState.Following)
                StateFollowing();

            float finalAttentionRadius = attentionRadius + (World.Player.HasDroneAttentionRadiusUpgrade ? 200 : 0);

            if (target==null)
            {
                setTargetTimer += 0.01f;
                if (setTargetTimer >= 1)
                {
                    float dist = finalAttentionRadius;
                    foreach (Monster monster in World.GameObjects.OfType<Monster>())
                    {
                        float dst = (Position - monster.Position).Length();
                        if (dst < dist && !World.PointOutOfView(monster.Position))
                        {
                            dist = dst;
                            target = monster;
                        }
                    }
                    setTargetTimer = 0;
                }
            }
            else
            {
                FlipX = target.Position.X - Position.X < 0;
                shootTimer += 0.02f;
                if (shootTimer >= 1)
                {
                    Vector2 bulletSpeed = new Vector2(5, (target.Position - Position).Angle()).ToCartesian();
                    World.AddObject(new PlayerBullet() { Speed = bulletSpeed }, Position);
                    shootTimer = 0;
                    Speed -= bulletSpeed * 0.2f;
                    Audio.Play("Audio/Combat/Gunshots/Laser/Laser_Shoot01");
                }
                //lose target
                if (World.PointOutOfView(target.Position) || target.HitPoints <= 0 || (Position - target.Position).Length() > finalAttentionRadius)
                    target = null;
            }

            checkDroneCollision++;
            List<Drone> drones = World.GameObjects.OfType<Drone>().ToList();
            if (checkDroneCollision == drones.Count())
            {
                for (var i = 0; i< drones.Count(); i++)
                {
                    for (var j = i + 1; j < drones.Count(); j++)
                    {
                        Drone a = drones[i];
                        Drone b = drones[j];
                        Vector2 d = (b.Position - a.Position);
                        d = d.ToPolar();
                        d.X -= 20;
                        if (d.X < 0)
                        {
                            d = d.ToCartesian();
                            a.Speed += d / 2;
                            b.Speed -= d / 2;
                        }
                    }
                }
                    checkDroneCollision = 0;
            }
        }

        void StateFollowing()
        {
            targetRot += 4;
            targetRot %= 360;

            acceleration = Vector2.Zero;

            //follow player
            targetPosition = World.Player.Position + new Vector2(-World.Player.GetFlip * 20, -20) + new Vector2(7, targetRot).ToCartesian() * new Vector2(0.6f, 1);

            FlipX = World.Player.Position.X > Position.X;

            //if ((targetPosition - Position).Length() > 0)
            {
                Vector2 speedPlus = (targetPosition - Position).ToPolar();
                //speedPlus.X = Math.Min(speedPlus.X * 0.01f, 0.3f);
                speedPlus.X *= 0.01f;
                speedPlus = speedPlus.ToCartesian();
                acceleration += speedPlus;
            }

            acceleration = acceleration.ToPolar();
            acceleration.X = Math.Min(maxAcceleration, acceleration.X);
            acceleration = acceleration.ToCartesian();
            Speed += acceleration;
        }

        public override void Draw()
        {
            ImageRotation = VectorExtensions.LengthDirectionY(15, targetRot * 2);
            Drawing.DrawRectangle(DrawBoundingBox, Color.Purple);
            base.Draw();
            Drawing.DrawSprite(CurrentSprite, DrawPosition, 0, CurrentSprite.Size * 0.5f * new Vector2(GetFlip, 1), null, MathHelper.ToRadians(ImageRotation) * GetFlip);// ImageScaling, Color.White, ImageRotation);
        }
    }
    public enum DroneState
    {
        Following, Attacking
    }
}
