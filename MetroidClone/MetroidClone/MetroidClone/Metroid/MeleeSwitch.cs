using System.Text;
using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class MeleeSwitch : PhysicsObject
    {
        bool activated = false;
        float shortest_Distance = 9000;
        int counter = 0;
        MeleeDoor linked_Door = new MeleeDoor();
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
                if (World.Player.CurrentWeapon == Weapon.Wrench)
                {
                    if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                    {
                        activated = true;
                    }
                }
            }
            if (activated)
            {
                if (counter == 0)
                {
                    // checks which door is closest and links the switch with that door
                    foreach (MeleeDoor door in World.GameObjects.OfType<MeleeDoor>().ToList())
                    {
                        float distance;
                        distance = Vector2.Distance(Position, door.Position);
                        if (distance < shortest_Distance)
                        {
                            linked_Door = door;
                            shortest_Distance = distance;
                        }
                    }

                }
                // opens the door
                if (counter < 120)
                {
                    linked_Door.Position.Y = linked_Door.Position.Y - World.Level.TileSize.Y / 60f;
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
                Drawing.DrawRectangle(TranslatedBoundingBox, Color.Yellow);
        }
    }
}

