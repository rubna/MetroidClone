using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class GunUpgrade : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-23, -12, 46, 24);
            SetSprite("Pickups/playergunUpgraded");
        }

        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                Audio.Play("Audio/PickUps/Powerup01");
                World.Player.CurrentWeapon = Weapon.Gun;
                Destroy();
                World.Player.Score += 200;
                World.Player.HasGunUpgrade = true;
                
            }
            base.Update(gameTime);
        }
    }
}
