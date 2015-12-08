using MetroidClone.Engine;
using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerMelee : PhysicsObject, IPlayerAttack
    {
        float removeCounter = 0f;
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -20, 20, 40);
            CollideWithWalls = false;
            Gravity = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            removeCounter += 0.1f;
            if (removeCounter >= 1)
                Destroy();
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Blue);
        }
    }
}
