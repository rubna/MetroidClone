using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class PushableBlock : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //chekcs if there is  a collission with the player and on wich sides that is on.
            foreach (Player player in World.GameObjects.OfType<Player>())
            {
                if(TranslatedBoundingBox.Intersects(player.TranslatedBoundingBox))
                {
                    if (player.Position.X < Position.X)
                        Position.X = Position.X + 1;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Blue);
        }
    }
}
