using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class WrenchPickup : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-23, -12, 46, 24);
            SetSprite("Pickups/wrench");
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                World.Tutorial.PickedUpWrench = true;
                Audio.Play("Audio/PickUps/Powerup02");
                World.Player.UnlockedWeapons.Remove(Weapon.Nothing);
                World.Player.UnlockedWeapons.Add(Weapon.Wrench);
                Destroy();
            }
            base.Update(gameTime);
        }
    }
}
