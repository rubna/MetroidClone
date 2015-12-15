using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class Switch : PhysicsObject
    {
        bool activated = false;
        public override void Create()
        {
            base.Create();
            Gravity = 0;
            BoundingBox = new Rectangle(-5, -3, 10, 15);
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (World.Player.CurrentWeapon == Weapon.Gun)
                {
                    if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                    {
                        activated = true;
                    }
                }
            }
            if (activated)
            {
                
                float shortest_Distance = 0;
                int counter = 0;
                Door linked_Door = new Door();
                foreach (Door door in World.GameObjects.OfType<Door>().ToList())
                {
                    float distance;
                    distance = Vector2.Distance(Position, door.Position);
                    if (shortest_Distance == 0)
                    {
                        linked_Door = door;
                        shortest_Distance = distance;
                    }
                    else
                    if (distance < shortest_Distance)
                    {
                        linked_Door = door;
                        shortest_Distance = distance;
                    }
                }
                if (counter < 120)
                {
                    linked_Door.Position.Y--;
                    counter++;
                }
            }
        }
        public override void Draw()
        {
            base.Draw();
            if (activated)
                Drawing.DrawRectangle(TranslatedBoundingBox, Color.DarkGreen);
            else
                Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
        }
    }
}
