using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    partial class Boss : Monster
    {
        //Damage of the bullets
        public int AttackDamage = 10;
        float AnimationRotation = 0;
        float shotAnimationTimer = 0;
        float baseScale = 1.5f;

        AnimationBone body, hipLeft, kneeLeft, hipRight, kneeRight, head,
            shoulderLeft, shoulderRight, elbowLeft, elbowRight, gun, gunNuzzle;

        Vector2 startingPos;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle((int)(-13 * baseScale), (int)(-37 * baseScale), 
                                        (int)(26 * baseScale), (int)(50 * baseScale));
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 120;
            Damage = 5; //Damage of the monster itself

            SpriteScale = 0.2f * baseScale;

            //animation rig
            body = new AnimationBone(this, new Vector2(0, -8 * baseScale));
            head = new AnimationBone(body, new Vector2(0, -30 * baseScale)) { DepthOffset = -2 };
            hipLeft = new AnimationBone(body, new Vector2(6 * baseScale, 4 * baseScale)) { DepthOffset = 2 };
            kneeLeft = new AnimationBone(hipLeft, new Vector2(0, 8 * baseScale)) { DepthOffset = -1 };
            hipRight = new AnimationBone(body, new Vector2(-6 * baseScale, 4 * baseScale)) { DepthOffset = 1 };
            kneeRight = new AnimationBone(hipRight, new Vector2(0, 8 * baseScale)) { DepthOffset = -1 };

            shoulderLeft = new AnimationBone(body, new Vector2(8 * baseScale, -24 * baseScale)) { DepthOffset = 2 };
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(13 * baseScale, 0)) { DepthOffset = -1 };
            shoulderRight = new AnimationBone(body, new Vector2(-8 * baseScale, -24 * baseScale)) { DepthOffset = -1 };
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-13 * baseScale, 0)) { DepthOffset = -1 };
            gun = new AnimationBone(elbowRight, new Vector2(-10 * baseScale, 0)) { DepthOffset = 1 };
            gunNuzzle = new AnimationBone(gun, new Vector2(15 * baseScale, -5 * baseScale));

            World.AddObject(body);
            body.SetSprite("Boss/Body");
            World.AddObject(head);
            head.SetSprite("Boss/Head");
            World.AddObject(hipLeft);
            hipLeft.SetSprite("Boss/LegL1");
            World.AddObject(kneeLeft);
            kneeLeft.SetSprite("Boss/LegL2");
            World.AddObject(hipRight);
            hipRight.SetSprite("Boss/LegR1");
            World.AddObject(kneeRight);
            kneeRight.SetSprite("Boss/LegR2");

            World.AddObject(shoulderLeft);
            shoulderLeft.SetSprite("Boss/ArmL1");
            World.AddObject(elbowLeft);
            elbowLeft.SetSprite("Boss/ArmL2");
            World.AddObject(shoulderRight);
            shoulderRight.SetSprite("Boss/ArmR1");
            World.AddObject(elbowRight);
            elbowRight.SetSprite("Boss/ArmR2");
            World.AddObject(gun);
            gun.SetSprite("Boss/Gun");
            World.AddObject(gunNuzzle);

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
            World.AddObject(new BossBullet(AttackDamage), gunNuzzle.Position);
        }

        void PlayAnimations()
        {
            AnimationRotation += 4;

            if (!OnGround)
            {
                PlayAnimationLegsInAir();
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsShooting((World.Player.Position - Position).Angle());
                else
                    PlayAnimationArmsIdle();
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
