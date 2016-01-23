using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    partial class ShootingMonster : Monster
    {
        //distance between player and monster
        float distance = 0;

        //time of shooting burst
        float shootTime = 0;
        
        //has a shot in the burst been fired yet
        bool shot1 = false, shot2 = false;
        
        //bullet damage
        public int AttackDamage = 5;
        float AnimationRotation = 0;
        float shotAnimationTimer = 0;

        AnimationBone body, hipLeft, kneeLeft, hipRight, kneeRight, head,
                    shoulderLeft, shoulderRight, elbowLeft, elbowRight, gun;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-13, -27, 26, 40);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 10;
            Damage = 5;
            ScoreOnKill = 20;

            SpriteScale = 0.2f;

            //animation rig
            body = new AnimationBone(this, new Vector2(0, -8));
            head = new AnimationBone(body, new Vector2(3, -28));
            hipLeft = new AnimationBone(body, new Vector2(6, 0)) { DepthOffset = 2 };
            kneeLeft = new AnimationBone(hipLeft, new Vector2(0, 8)) { DepthOffset = -1 };
            hipRight = new AnimationBone(body, new Vector2(-6, 0)) { DepthOffset = 1 };
            kneeRight = new AnimationBone(hipRight, new Vector2(0, 8)) { DepthOffset = -1 };

            shoulderLeft = new AnimationBone(body, new Vector2(6, -20));
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(16, 0));
            shoulderRight = new AnimationBone(body, new Vector2(-6, -20));
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-16, 0));
            gun = new AnimationBone(elbowRight, new Vector2(-10, 0)) { DepthOffset = -1 };

            World.AddObject(body);
            body.SetSprite("Enemy1/Body");
            World.AddObject(head);
            head.SetSprite("Enemy1/Head");
            World.AddObject(hipLeft);
            hipLeft.SetSprite("Enemy1/LLeg");
            World.AddObject(kneeLeft);
            kneeLeft.SetSprite("Enemy1/LFoot");
            World.AddObject(hipRight);
            hipRight.SetSprite("Enemy1/RLeg");
            World.AddObject(kneeRight);
            kneeRight.SetSprite("Enemy1/RFoot");

            World.AddObject(shoulderLeft);
            shoulderLeft.SetSprite("Enemy1/LArm1");
            World.AddObject(elbowLeft);
            elbowLeft.SetSprite("Enemy1/LArm2");
            World.AddObject(shoulderRight);
            shoulderRight.SetSprite("Enemy1/RArm1");
            World.AddObject(elbowRight);
            elbowRight.SetSprite("Enemy1/RArm2");
            World.AddObject(gun);
            gun.SetSprite("Enemy1/Enemygun");

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AnimationRotation += 4;

            if (Input.KeyboardCheckPressed(Keys.LeftAlt))
            {
                Speed.Y = -5;
            }

            if (!OnGround)
            {
                PlayAnimationLegsInAir();
            }
            else
            if (Input.KeyboardCheckDown(Keys.Space))
            {
                PlayAnimationLegsWalking();
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsShooting((World.Player.Position - Position).Angle());
                else
                    PlayAnimationArmsWalking();
                AnimationRotation += 4;
            }
            else
            {
                PlayAnimationLegsIdle();
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsShooting((World.Player.Position - Position).Angle());
                else
                    PlayAnimationArmsIdle();
            }

            //has shot
            if (shotAnimationTimer > 0)
                shotAnimationTimer -= 0.05f;
            
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
            shotAnimationTimer = 1;
            FlipX = (Position.X - World.Player.Position.X) > 0;
            World.AddObject(new MonsterBullet(AttackDamage), Position);
        }

        public override void Draw()
        {
            base.Draw();
            //Drawing.DrawRectangle(DrawBoundingBox, Color.Purple);
        }
    }
}
