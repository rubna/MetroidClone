using MetroidClone.Engine;
using Microsoft.Xna.Framework;

// creates a gameobject which can drop if you kill a monster and gives you 1 extra rocket
namespace MetroidClone.Metroid
{
    class RocketAmmo : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-8, -3, 16, 6);

            SetSprite("Pickups/ammopickup");
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                //The ammo limit is a soft limit, not a hard one.
                //if (World.Player.RocketAmmo < World.Player.MaximumRocketAmmo)
                {
                    Audio.Play("Audio/PickUps/collectscraporrocket");
                    World.Tutorial.AmmoCollected = true;
                    World.Player.RocketAmmo++;
                    Destroy();
                }
            }
            base.Update(gameTime);

        }
    }
}
