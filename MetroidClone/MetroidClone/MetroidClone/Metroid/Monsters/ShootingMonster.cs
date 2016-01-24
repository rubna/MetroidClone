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
        //Damage of the bullets
        public int AttackDamage = 5;
        float AnimationRotation = 0;
        float shotAnimationTimer = 0;

        AnimationBone body, hipLeft, kneeLeft, hipRight, kneeRight, head,
            shoulderLeft, shoulderRight, elbowLeft, elbowRight, gun;

        Vector2 startingPos;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-13, -27, 26, 40);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 5;
            Damage = 5; //Damage of the monster itself

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


            State = MonsterState.ChangeState;
            StateTimer = 60;

            Gravity = 0.2f;

            startingPos = Position;
        }

        public override void Update(GameTime gameTime)
        {
            //Only update if we're inside the view
            if (!World.PointOutOfView(Position))
            {
                base.Update(gameTime);

                //has shot
                if (shotAnimationTimer > 0)
                    shotAnimationTimer -= 0.05f;

                DoBasicAI();

                //play proper animations
                PlayAnimations();
                Visible = true;
            }
            else
            //not in room
            {
                Visible = false;
                StateTimer = 0;
                if (World.PointOutOfView(Position, -50)) //If the position is very near the view edge, reset it.
                    Position = startingPos;
            }
        }

        protected override void Attack()
        {
            shotAnimationTimer = 1;
            FlipX = (Position.X - World.Player.Position.X) > 0;
            World.AddObject(new MonsterBullet(AttackDamage), Position);
        }

        void PlayAnimations()
        {
            AnimationRotation += 4;

            if (!OnGround)
            {
                PlayAnimationLegsInAir();
            }
            else if (Math.Abs(Speed.X) > 0.1f)
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
        }

        public override void Destroy()
        {
            base.Destroy();
            body.Destroy();
            hipLeft.Destroy();
            kneeLeft.Destroy();
            hipRight.Destroy();
            kneeRight.Destroy();
            head.Destroy();
            shoulderLeft.Destroy();
            shoulderRight.Destroy();
            elbowLeft.Destroy();
            elbowRight.Destroy();
            gun.Destroy();
        }
    }
}
