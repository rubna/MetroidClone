using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;

//creates a healthdrop which the player can pick up to get hitpoints back
namespace MetroidClone.Metroid
{
    class HealthDrop : PhysicsObject
    {
        int hitPointsFromHealthDrop;

        public HealthDrop(int monsterDifficulty)
        {
            hitPointsFromHealthDrop = Math.Max(10, monsterDifficulty);
        }

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-6, -9, 12, 20);
            SetSprite("Pickups/healthdrop");
        }

        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                if (World.Player.HitPoints < World.Player.MaxHitPoints)
                {
                    World.Player.HitPoints = Math.Min(World.Player.HitPoints + hitPointsFromHealthDrop, World.Player.MaxHitPoints);
                    World.Tutorial.HealthBonusCollected = true;
                    Destroy();
                }
            }
            base.Update(gameTime);

        }
    }
}
