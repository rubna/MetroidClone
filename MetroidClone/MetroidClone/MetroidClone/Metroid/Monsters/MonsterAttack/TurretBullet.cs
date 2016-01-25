using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    //Bullets shot by the torret
    class TurretBullet : MonsterBullet
    {
        public TurretBullet(int damage) : base(damage)
        {
            //Do nothing extra in this constructor.
        }

        public override void Create()
        {
            base.Create();
            SetSprite("Turret/Turretbullet");
            BoundingBox = new Rectangle(-4, -3, 8, 6);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
