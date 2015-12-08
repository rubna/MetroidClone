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
        PhysicsObject target = null;
        Vector2 targetPosition = Vector2.Zero;
        public DroneState CurrentState = DroneState.Following;
        float targetRot = 0f;
        List<Vector2> avoidPoints = new List<Vector2>();
        float createPointCounter = 0f;

        public override void Create()
        {
            base.Create();
            Gravity = 0;
            Friction = Vector2.One * 0.93f;
            BoundingBox = new Rectangle(-6, -6, 12, 12);
            WallBounce = Vector2.One * 0.9f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (CurrentState == DroneState.Following)
                StateFollowing();


            if (createPointCounter >= 1)
            {
                Rectangle avoidBox = BoundingBox;
                avoidBox.Inflate(2, 2);
                if (InsideWall(Position, avoidBox))
                    avoidPoints.Add(Position);
                createPointCounter = 0;
            }
            else
                createPointCounter += 0.1f;

        }

        void StateFollowing()
        {
            targetRot += 4;
            targetRot %= 360;

            //follow player
            targetPosition = World.Player.Position + new Vector2(-World.Player.GetFlip * 20, -20) + new Vector2(7, targetRot).ToCartesian() * new Vector2(0.6f, 1);
            //if ((targetPosition - Position).Length() > 0)
            {
                Vector2 speedPlus = (targetPosition - Position).ToPolar();
                //speedPlus.X = Math.Min(speedPlus.X * 0.01f, 0.3f);
                speedPlus.X *= 0.01f;
                speedPlus = speedPlus.ToCartesian();
                Speed += speedPlus;
            }

            foreach (Vector2 point in avoidPoints)
            {
                if ((point - Position).Length() < 30)
                {
                    Vector2 speedPlus = (Position - point).ToPolar();
                    speedPlus.X = 30 - speedPlus.X;
                    speedPlus.X *= 0.01f;
                    speedPlus = speedPlus.ToCartesian();
                    Speed += speedPlus;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            foreach (Vector2 point in avoidPoints)
            {
                Drawing.DrawCircle(point, 2, Color.Blue, 4);
            }
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Purple);
        }
    }
    public enum DroneState
    {
        Following, Attacking
    }
}
