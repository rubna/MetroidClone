using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class HealthDrop : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-5, -3, 10, 6);
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                if (World.Player.HitPoints < 100)
                {
                    World.Player.HitPoints = World.Player.HitPoints + 5;
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
