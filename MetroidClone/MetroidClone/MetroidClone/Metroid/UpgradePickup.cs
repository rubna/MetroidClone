using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    abstract class UpgradePickup : PhysicsObject
    {
        //The text when the upgrade happens
        public abstract string Text { get; }

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-15, -15, 30, 30);
            SetSprite("Pickups/upgradedrop");
        }

        public override void Update(GameTime gameTime)
        {
            if (CollidesWith(Position, World.Player))
            {
                Audio.Play("Audio/PickUps/upgrade");
                DoUpgrade();
                Destroy();
                World.Player.Score += 30;
                World.Tutorial.ShowMessage(Text);
            }
            base.Update(gameTime);
        }

        //This should define the behaviour when the upgrade happens
        public abstract void DoUpgrade();
    }
}
