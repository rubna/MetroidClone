using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class RocketPickup : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-23, -12, 46, 24);
            SetSprite("Pickups/playerrocket");
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                World.Tutorial.PickedUpRocket = true;
                Audio.Play("Audio/PickUps/Powerup03");
                World.Player.UnlockedWeapons.Remove(Weapon.Nothing);
                World.Player.UnlockedWeapons.Add(Weapon.Rocket);
                World.Player.CurrentWeapon = Weapon.Rocket;
                Destroy();
                World.Player.Score += 500;
            }
            base.Update(gameTime);
        }
    }
}
