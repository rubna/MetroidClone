﻿using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class PlayerBullet : PhysicsObject, IPlayerAttack
    {
        private Vector2 direction;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-4, -4, 8, 8);
            Friction.X = 1;
            Gravity = 0;
            CollideWithWalls = false;
            if (Input.ControllerInUse)
            {
                Vector2 dir = Input.ThumbStickCheckDirection(false);
                dir.Y = -dir.Y;
                dir.Normalize();
                Speed = 5 * dir;
            }
            else
            {
                Speed = Input.MouseCheckUnscaledPosition(Drawing).ToVector2() - DrawPosition;
                Speed.Normalize();
                Speed *= 5;
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Speed.X < 0.01f || Position.Y != PositionPrevious.Y)
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(DrawBoundingBox, Color.Blue);
            base.Draw();
        }
    }
}
