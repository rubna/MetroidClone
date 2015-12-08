using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid.Monsters
{
    class TestMonster : Monster
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 10;
            Damage = 5;
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
        }
    }
}
