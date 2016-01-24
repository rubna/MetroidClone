using MetroidClone.Engine;
using MetroidClone.Metroid.Abstract;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerMelee : PhysicsObject, IPlayerAttack
    {
        public float Damage => 1;

        float removeCounter = 0f;
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-30, -40, 60, 60);
            CollideWithWalls = false;
            Gravity = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = World.Player.Position + World.Player.GetFlip * Vector2.UnitX * 20;
            removeCounter += 0.1f;

            //Collide with doors
            ISolid doorCollision = GetCollisionWithSolid<MeleeDoor>(TranslatedBoundingBox);

            if (doorCollision != null)
            {
                (doorCollision as Door).Activated = true;
                World.Tutorial.WrenchDoorOpened = true;
            }

            if (removeCounter >= 1)
                Destroy();
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Blue);
        }
    }
}
