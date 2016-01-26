using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System.Linq;

namespace MetroidClone.Metroid
{
    //A door that closes when you move past it. It also closes all other boss doors.
    class BossDoor : BoxObject, ISolid
    {
        public bool Closed = false, Activated = false, HasNeverBeenActivated = true;
        int switchTimer = 0;

        const int switchTime = 30;

        protected string standardSprite => "doorspriteopen";
        protected string closedSprite => "doorspriteclosedboss";

        public override void Create()
        {
            base.Create();
            Depth = -20;
            BoundingBox = new Rectangle(0, 0, World.TileWidth / 4, World.TileHeight * 2);

            SetSprite(standardSprite);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Activated)
            {
                if (Closed)
                {
                    // opens the door
                    if (switchTimer > 0)
                    {
                        switchTimer--;
                    }
                    else
                    {
                        Activated = false;
                        switchTimer = 0;
                        Closed = false;
                    }
                }
                else
                {
                    // close the door
                    if (switchTimer < switchTime)
                    {
                        switchTimer++;
                    }
                    else
                    {
                        Activated = false;
                        switchTimer = switchTime;
                        Closed = true;
                    }
                }
            }
            else if (HasNeverBeenActivated)
            {
                //Activate the door if the player is to the right of it
                if (!World.PointOutOfView(Position, 0))
                {
                    if (World.Player.Position.X > Position.X + 28)
                    {
                        //Activate all boss doors
                        foreach (BossDoor door in World.GameObjects.OfType<BossDoor>())
                        {
                            if (door.HasNeverBeenActivated)
                                door.Activated = true;
                            door.HasNeverBeenActivated = false;
                        }

                        //Teleport drones to player (to avoid them being stuck in the door)
                        foreach (Drone drone in World.GameObjects.OfType<Drone>())
                        {
                            drone.Position = World.Player.Position;
                        }

                        //Also activated boss portals
                        foreach (Portal portal in World.GameObjects.OfType<Portal>())
                        {
                            portal.Activated = true;
                        }
                    }
                }
            }
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            if (Closed || Activated)
                return box.Intersects(TranslatedBoundingBox);
            else
                return false;
        }

        public override void Draw()
        {
            Vector2? drawSize = ImageScaling * new Vector2(BoundingBox.Height * (34f / 128f), BoundingBox.Height);

            Drawing.DrawSprite(standardSprite, DrawPosition - new Vector2(World.TileWidth / 4, 0f), size: drawSize);

            if (Closed || switchTimer > 0)
                Drawing.DrawSprite(closedSprite, DrawPosition - new Vector2(World.TileWidth / 4, 0f), color: Color.White * (switchTimer / (float) switchTime),
                    size: drawSize);
        }
    }
}
