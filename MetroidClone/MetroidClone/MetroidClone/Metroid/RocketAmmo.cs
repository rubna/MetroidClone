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
            BoundingBox = new Rectangle(-5, -3, 10, 6);
        }
        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                if (World.Player.RocketAmmo < World.Player.MaximumRocketAmmo)
                {
                    World.Tutorial.AmmoCollected = true;
                    World.Player.RocketAmmo++;
                    Destroy();
                }
            }
            base.Update(gameTime);

        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Yellow);
        }
    }
}
