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
        //distance between player and monster
        float distance = 0;

        //time of shooting burst
        float shootTime = 0;
        
        //has a shot in the burst been fired yet
        bool shot1 = false, shot2 = false;
        
        //bullet damage
        public int AttackDamage = 5;

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

            //if the player is in range, the monster will shoot every second. if the player gets to far away, the attack cooldown
            //will decrease, so that the monster will still attack if the player gets in range and out range over and over
            if (distance <= 10 * World.TileWidth)
            {
                shootTime++;
                if (shootTime >= 60)
                {
                    Attack();
                    shootTime = 0;
                }
            }
            else
            {
                if (shootTime > 0)
                    shootTime--;
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
