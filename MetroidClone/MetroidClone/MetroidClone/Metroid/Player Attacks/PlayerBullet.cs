﻿using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class PlayerBullet : PhysicsObject, IPlayerAttack
    {
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

            ISolid doorCollision = GetCollisionWithSolid<GunDoor>(TranslatedBoundingBox);
            
            if (doorCollision != null)
            {
                (doorCollision as Door).Activated = true;
            }

            if (InsideWall(TranslatedBoundingBox))
                Destroy();

            if (World.PointOutOfView(Position, -10))
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(DrawBoundingBox, Color.Blue);
            base.Draw();
        }
    }
}
