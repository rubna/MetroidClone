﻿using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class WrenchPickup : PhysicsObject
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
                World.Player.UnlockedWeapons.Remove(Weapon.Nothing);
                World.Player.UnlockedWeapons.Add(Weapon.Wrench);
                World.Player.CurrentWeapon = Weapon.Wrench;
                Destroy();
            }
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Gray);
        }
    }
}
