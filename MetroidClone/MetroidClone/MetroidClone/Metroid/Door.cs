using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class Door : PhysicsObject, ISolid
    {
        public override void Create()
        {
            base.Create();
            CollideWithWalls = false;
            Gravity = 0;
            BoundingBox = new Rectangle(0, 0, World.Level.TileSize.X / 3, World.Level.TileSize.Y * 2);
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(TranslatedBoundingBox);
        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Blue);
        }


    }
}
