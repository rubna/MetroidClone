using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class DestroyableDoor : PhysicsObject, ISolid
    {
        public override void Create()
        {
            base.Create();
            CollideWithWalls = false;
            Gravity = 0;
            BoundingBox = new Rectangle(-5, -3, World.Level.TileSize.X / 3, World.Level.TileSize.Y * 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        
            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (World.Player.CurrentWeapon == Weapon.Rocket)
                {
                    // checks if the rocket hits the door
                    if (TranslatedBoundingBox.X - 4 == attack.TranslatedBoundingBox.X && attack.TranslatedBoundingBox.Y >= TranslatedBoundingBox.Y && attack.TranslatedBoundingBox.Y <= TranslatedBoundingBox.Y + 2 * World.Level.TileSize.Y || TranslatedBoundingBox.X + World.Level.TileSize.X / 3 + 4 == attack.TranslatedBoundingBox.X && attack.TranslatedBoundingBox.Y >= TranslatedBoundingBox.Y && attack.TranslatedBoundingBox.Y <= TranslatedBoundingBox.Y + 2 * World.Level.TileSize.Y)
                    {
                        Destroy();
                        attack.Destroy();
                    }
                }
            }
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(TranslatedBoundingBox);
        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Silver);
            
        }
    }
}
