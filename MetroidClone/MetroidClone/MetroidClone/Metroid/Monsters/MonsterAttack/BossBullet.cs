using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class BossBullet : PhysicsObject, IMonsterAttack
    {
        public int Damage { get; }
        public const int baseSpeed = 5;

        public BossBullet(int damage)
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
            SetSprite("Boss/Bullet");
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
            base.Draw();
        }
    }
}
