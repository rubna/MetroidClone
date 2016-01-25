using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;
using System;

namespace MetroidClone.Metroid
{
    class PlayerBullet : PhysicsObject, IPlayerAttack
    {
        //The gun upgrade makes the player deal more damage.
        public float Damage
        {
            get { return World.Player.HasGunUpgrade ? 1.5f : 1f; }
        }

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-4, -4, 8, 8);
            Friction.X = 1;
            Gravity = 0;
            CollideWithWalls = false;

            SetSprite("Robot/playergunbullet");
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ImageRotation = MathHelper.ToRadians(VectorExtensions.Angle(Speed));

            ISolid doorCollision = GetCollisionWithSolid<GunDoor>(TranslatedBoundingBox);
            
            if (doorCollision != null)
            {
                if (!(doorCollision as Door).Activated)
                {
                    (doorCollision as Door).Activated = true;
                    World.Player.Score += 10;
                    World.Tutorial.GunDoorOpened = true;
                }
            }

            if (InsideWall(TranslatedBoundingBox))
                Destroy();

            if (World.PointOutOfView(Position, -10))
                Destroy();
        }

        public override void Draw()
        {
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, DrawPosition, (int)CurrentImage, size: new Vector2(13, 6), rotation: ImageRotation); //Draw the current image of the sprite.
        }
    }
}
