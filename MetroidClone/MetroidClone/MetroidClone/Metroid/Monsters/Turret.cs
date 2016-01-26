using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    class Turret : Monster
    {
        //Damage of the bullets of the monster
        public int AttackDamage = 5;
        float shotTimer = 0;
        int stayAtRotationTime = 0;
        float rotation = 0;
        Vector2 rotationSizeModVector; //How we should scale the turret.
        float firstChargeTimer = 0;
        Vector2 gunOffset = new Vector2(12, 10);

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-15, -15, 30, 30);
            HitPoints = 10;
            Damage = 0; //The turret itself does no damage
            Gravity = 0;
            
            SpriteScale = 0.2f;

            SetSprite("Turret/Turretbase");

            rotationSizeModVector = new Vector2(1f);

            ScrapDropModifier = 1.5f;
        }

        public override void Update(GameTime gameTime)
        {
            //Only update if we're inside the view
            if (!World.PointOutOfView(Position))
            {
                base.Update(gameTime);

                if (firstChargeTimer < 2)
                    firstChargeTimer += 0.05f;
                else
                {
                    //has shot
                    if (shotTimer > 0)
                        shotTimer -= 0.05f;
                    else //Shoot!
                    {
                        if (CanReachPlayer())
                            Attack();
                    }
                }

                //We should stay at the same rotation for a while after having shot
                if (stayAtRotationTime > 0)
                    stayAtRotationTime--;
                else
                {
                    //rotate the turret to face the player.
                    if (World.Player.Position.X != Position.X)
                    {
                        rotation = (float)Math.Atan(((World.Player.Position.Y - Position.Y)) / (World.Player.Position.X - Position.X));

                        if (World.Player.Position.X > Position.X)
                        {
                            rotationSizeModVector = new Vector2(1, -1);
                            rotation = rotation - (float)Math.PI;
                        }
                        else
                            rotationSizeModVector = new Vector2(1, 1);
                    }
                }
            }
            else
                firstChargeTimer = 0;
        }

        //Shoot a bullet
        protected override void Attack()
        {
            shotTimer = 2; //It takes some time to recharge
            FlipX = (Position.X - World.Player.Position.X) > 0;
            World.AddObject(new TurretBullet(AttackDamage), Position + gunOffset);
            stayAtRotationTime = 5;
        }

        //Draw the turret
        public override void Draw()
        {
            //Drawing.DrawRectangle(DrawBoundingBox, Color.Blue);

            base.Draw();
            Drawing.DrawSprite("Turret/Turretgun", DrawPosition + gunOffset, 0,
                size: new Vector2(BoundingBox.Width + 10, (BoundingBox.Width + 10) / 3) * rotationSizeModVector, rotation: rotation);
        }
    }
}
