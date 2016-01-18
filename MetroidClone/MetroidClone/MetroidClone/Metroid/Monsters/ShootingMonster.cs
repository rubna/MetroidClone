using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;


namespace MetroidClone.Metroid
{
    class ShootingMonster : Monster
    {
        float distance = 0;
        float shootTime = 0;
        bool shot1 = false, shot2 = false;
        public int AttackDamage = 5;

        const float jumpSpeed = 8f;
        const float baseSpeed = 0.4f;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 10;
            Damage = 5;
            ScoreOnKill = 20;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            //calculates the distance between monster and player. if player is close enough, the monster will attack player
            distance = (Position - World.Player.Position).Length();
            if (distance <= 10 * World.TileWidth)
            {
                shootTime++;
                if (shootTime >= 40 && !shot1)
                {
                    Attack();
                    shot1 = true;
                }
                if (shootTime >= 50 && !shot2)
                {
                    Attack();
                    shot2 = true;
                }
                if (shootTime >= 60)
                {
                    Attack();
                    shootTime = 0;
                    shot1 = false;
                    shot2 = false;
                }

                //Move if needed

                //Left
                if (World.Player.Position.X < Position.X)
                    Speed.X -= baseSpeed;
                //Right
                else if (World.Player.Position.X > Position.X)
                    Speed.X += baseSpeed;

                //Jump
                if (World.Player.Position.Y < Position.Y - 20 && OnGround)
                {
                    Speed.Y = -jumpSpeed;
                }
            }
            else
            {
                shootTime = 0;
                shot1 = false;
                shot2 = false;
            }
        }

        void Attack()
        {
            World.AddObject(new MonsterBullet(AttackDamage), Position);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Purple);
        }
    }
}
