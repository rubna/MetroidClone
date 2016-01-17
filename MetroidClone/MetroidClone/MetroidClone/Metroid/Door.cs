using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    abstract class Door : BoxObject, ISolid
    {
        public bool Activated = false;
        int activatedTime = 0;

        const float deactivateTime = 30f;

        abstract protected string standardSprite { get; }
        abstract protected string closedSprite { get; }

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
                // opens the door
                if (activatedTime < deactivateTime)
                {
                    activatedTime++;
                }
            }
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            if (!Activated || activatedTime < deactivateTime)
                return box.Intersects(TranslatedBoundingBox);
            else
                return false;
        }

        public override void Draw()
        {
            Vector2? drawSize = ImageScaling * new Vector2(BoundingBox.Height * (34f / 128f), BoundingBox.Height);

            Drawing.DrawSprite(standardSprite, DrawPosition - new Vector2(World.TileWidth / 4, 0f), size: drawSize);

            if (!Activated || activatedTime < deactivateTime)
                Drawing.DrawSprite(closedSprite, DrawPosition - new Vector2(World.TileWidth / 4, 0f), color: Color.White * ((deactivateTime - activatedTime) / deactivateTime),
                    size: drawSize);
        }
    }
}
