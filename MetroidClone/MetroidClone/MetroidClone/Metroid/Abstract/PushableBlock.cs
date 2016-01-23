using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class PushableBlock : PhysicsObject, ISolid
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
        }

        /*public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //chekcs if there is  a collission with the player and on wich sides that is on.
            foreach (Player player in World.GameObjects.OfType<Player>())
            {
                if(CollidesWith(0, 0, player))
                {
                    if (player.Position.X < Position.X)
                        Position.X = Position.X + 1;
                    else
                        Position.X = Position.X - 1;
                }
            }
        }
        */
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.SaddleBrown);
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(TranslatedBoundingBox);
        }
    }
}
