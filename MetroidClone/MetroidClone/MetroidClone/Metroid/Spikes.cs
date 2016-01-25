using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class Spikes : BoxObject
    {
        public int Damage = 5;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(0, 0, World.TileWidth, (int) (World.TileHeight * (28f / 128f)));
            SetSprite("Spikes");
        }
    }
}
