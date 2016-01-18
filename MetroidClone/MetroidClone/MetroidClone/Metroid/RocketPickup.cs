using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class RocketPickup : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-5, -3, 10, 6);
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                Audio.Play("Audio/PickUps/Powerup03");
                World.Player.UnlockedWeapons.Remove(Weapon.Nothing);
                World.Player.UnlockedWeapons.Add(Weapon.Rocket);
                World.Player.CurrentWeapon = Weapon.Rocket;
                Destroy();
            }
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Green);
        }
    }
}
