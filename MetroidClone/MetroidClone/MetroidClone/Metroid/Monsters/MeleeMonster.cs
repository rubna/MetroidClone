using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    class MeleeMonster : Monster
    {
        public int AttackDamage = 5;
        float AnimationRotation = 0;

        AnimationBone body, legs, headLeft, headRight,
            shoulderLeft, shoulderRight, elbowLeft, elbowRight;

        Vector2 startingPos;

        public override void Create()
        {
            BaseSpeed = 0.5f; //This monster is quite fast

            base.Create();
            BoundingBox = new Rectangle(-13, -30, 26, 36);
            SpeedOnHit = new Vector2(2, -1);
            HitPoints = 7;
            Damage = AttackDamage;

            SpriteScale = 0.15f;

            //animation rig
            legs = new AnimationBone(this, new Vector2(6, 0)) { DepthOffset = 2 };
            body = new AnimationBone(legs, new Vector2(-6, 0));
            headLeft = new AnimationBone(body, new Vector2(10, -22)) { DepthOffset = 1 };
            headRight = new AnimationBone(body, new Vector2(1, -22)) { DepthOffset = 1 };

            shoulderLeft = new AnimationBone(body, new Vector2(12, -17)) { DepthOffset = 1 };
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(13, 0)) { DepthOffset = 1 };
            shoulderRight = new AnimationBone(body, new Vector2(-2, -17)) { DepthOffset = -2 };
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-13, 0)) { DepthOffset = 1 };

            World.AddObject(body);
            body.SetSprite("Enemy2/Body");
            World.AddObject(headLeft);
            headLeft.SetSprite("Enemy2/LHead");
            World.AddObject(headRight);
            headRight.SetSprite("Enemy2/RHead");
            World.AddObject(legs);
            legs.SetSprite("Enemy2/Legs");

            World.AddObject(shoulderLeft);
            shoulderLeft.SetSprite("Enemy2/LArm1");
            World.AddObject(elbowLeft);
            elbowLeft.SetSprite("Enemy2/LArm2");
            World.AddObject(shoulderRight);
            shoulderRight.SetSprite("Enemy2/RArm1");
            World.AddObject(elbowRight);
            elbowRight.SetSprite("Enemy2/RArm2");


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
                DoBasicAI(false);

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

        public override void Draw()
        {
            Rectangle box = BoundingBox;
            box.Offset(DrawPosition.ToPoint());
            //Drawing.DrawRectangle(box, Color.Red);
            base.Draw();
        }

        void PlayAnimations()
        {
            //TODO
            AnimationRotation += 4;

            if (!OnGround)
                PlayAnimationInAir();
            else 
            if (Math.Abs(Speed.X) > 0.1f)
            {
                AnimationRotation += 8;
                PlayAnimationWalking();
            }
            else
            {
                AnimationRotation += 4;
                PlayAnimationIdle();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            body.Destroy();
            legs.Destroy();
            headLeft.Destroy();
            headRight.Destroy();
            shoulderLeft.Destroy();
            shoulderRight.Destroy();
            elbowLeft.Destroy();
            elbowRight.Destroy();
        }

        void PlayAnimationIdle()
        {
            shoulderLeft.TargetRotation = -10 + VectorExtensions.LengthDirectionX(3, AnimationRotation);
            elbowLeft.TargetRotation = -80 + VectorExtensions.LengthDirectionX(7, AnimationRotation);
            shoulderRight.TargetRotation = 10 - VectorExtensions.LengthDirectionX(3, AnimationRotation + 45);
            elbowRight.TargetRotation = 80 - VectorExtensions.LengthDirectionX(8, AnimationRotation + 45);
            headLeft.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation + 180);
            headRight.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation + 120);
            body.TargetRotation = 0;
        }

        void PlayAnimationWalking()
        {
            body.TargetRotation = 10 + VectorExtensions.LengthDirectionX(3, AnimationRotation);

            shoulderLeft.TargetRotation = -10 + VectorExtensions.LengthDirectionX(9, AnimationRotation * 2);
            elbowLeft.TargetRotation = -80 + VectorExtensions.LengthDirectionX(28, AnimationRotation * 2);
            shoulderRight.TargetRotation = 160 - VectorExtensions.LengthDirectionX(8, AnimationRotation * 2 + 45);
            elbowRight.TargetRotation = -80 - VectorExtensions.LengthDirectionX(30, AnimationRotation * 2 + 45);

            headLeft.TargetRotation = 0;
            headRight.TargetRotation = 0;
        }

        void PlayAnimationInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 2);
            factor += 2;
            factor /= 4;

            legs.TargetRotation = 10 - factor * 10;
            body.TargetRotation = -legs.TargetRotation;

            shoulderLeft.TargetRotation = 60 - factor * 80;
            elbowLeft.TargetRotation = -10 + factor * 40;
            shoulderRight.TargetRotation = -70 + factor * 90;
            elbowRight.TargetRotation = 10 - factor * 40;

            headLeft.TargetRotation = 0;
            headRight.TargetRotation = 0;
        }
    }
}
