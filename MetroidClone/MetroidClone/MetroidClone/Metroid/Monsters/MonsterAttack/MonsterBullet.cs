using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class MonsterBullet : PhysicsObject, IMonsterAttack
    {
        public int Damage;
        public const int baseSpeed = 5;

        public MonsterBullet(int damage)
        {
            Damage = damage;
        }
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-4, -4, 8, 8);
            Friction.X = 1;
            Gravity = 0;
            CollideWithWalls = false;
            Vector2 dir = World.Player.Position - Position;
            dir.Normalize();
            Speed = baseSpeed * dir;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (InsideWall(TranslatedBoundingBox))
                Destroy();
            if (World.PointOutOfView(Position))
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(DrawBoundingBox, Color.Red);
            base.Draw();
        }
    }
}
