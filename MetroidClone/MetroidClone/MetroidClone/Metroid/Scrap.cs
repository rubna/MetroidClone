using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class Scrap : Collectible
    {
        public int ScrapAmount;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            ScrapAmount = World.Random.Next(5, 11);
            SetSprite("Scrap/Scrap" + World.Random.Next(1, 7));
        }

        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                World.Tutorial.ScrapCollected = true;
                World.Player.CollectedScrap += ScrapAmount;
                Destroy();
            }
            base.Update(gameTime);
        }
    }
}
