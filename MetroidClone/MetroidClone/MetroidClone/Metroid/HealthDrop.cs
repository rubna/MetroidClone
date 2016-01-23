using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;

namespace MetroidClone.Metroid
{
    class HealthDrop : PhysicsObject
    {
        int hitPointsFromHealthDrop;

        public HealthDrop(int monsterDifficulty)
        {
            hitPointsFromHealthDrop = monsterDifficulty;
        }

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-5, -3, 10, 6);
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                if (World.Player.HitPoints < World.Player.MaxHitPoints)
                {
                    World.Player.HitPoints = Math.Min(World.Player.HitPoints + hitPointsFromHealthDrop, hitPointsFromHealthDrop);
                    Destroy();
                }
            }
            base.Update(gameTime);

        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Pink);
        }
    }
}
