﻿using MetroidClone.Engine;
using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerExplosion : PhysicsObject, IPlayerAttack
    {
        public float Damage => 1;

        float radius = 50;
        float destroyTimer = 0f;

        public override void Create()
        {
            base.Create();
            Gravity = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            destroyTimer += 0.1f;
            if (destroyTimer >= 1)
                Destroy();
        }

        public override void Draw()
        {
            if (Visible)
            Drawing.DrawCircle(Position, radius, Color.Black);
            base.Draw();
        }
    }
}
