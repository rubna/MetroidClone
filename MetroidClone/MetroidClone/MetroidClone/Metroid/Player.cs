using MetroidClone.Engine;
using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class Player : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            OriginalBoundingBox = new Rectangle(0, 0, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            //move around
            if (Input.KeyboardCheckDown(Keys.Left))
                Speed.X -= 0.5f;
            if (Input.KeyboardCheckDown(Keys.Right))
                Speed.X += 0.5f;
            if (Input.KeyboardCheckPressed(Keys.Up))
                Speed.Y = -4.5f;

            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            Sprite sprite = Assets.GetSprite("youSpriteWowCoderArt");
            Drawing.DrawSprite(sprite, new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y), size: new Vector2(BoundingBox.Width, BoundingBox.Height));
        }
    }
}
