using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class GunPickup : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-23, -12, 46, 24);
            SetSprite("Pickups/playergun");
        }

        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                Audio.Play("Audio/PickUps/Powerup01");
                World.Player.UnlockedWeapons.Remove(Weapon.Nothing);
                World.Player.UnlockedWeapons.Add(Weapon.Gun);
                World.Player.CurrentWeapon = Weapon.Gun;
                Destroy();
            }
            base.Update(gameTime);
        }
    }
}
