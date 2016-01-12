using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MetroidClone.Metroid
{
    class Smoke : GameObject
    {
        int smokeTime;

        public Smoke()
        {
            smokeTime = 0;
        }

        public override void Create()
        {
            base.Create();

            SetSprite("placeholders/smoke");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (smokeTime > 60)
                Destroy();
            else
                smokeTime++;
        }

        public override void Draw()
        {
            Drawing.DrawSprite(CurrentSprite, DrawPosition, color: new Color(150, 150, 150, 255 - smokeTime*5)); //Draw the current image of the sprite.
        }
    }
}
