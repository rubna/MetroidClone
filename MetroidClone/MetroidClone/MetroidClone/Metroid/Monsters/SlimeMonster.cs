using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    class SlimeMonster : Monster
    {
        public int AttackDamage = 5;
        float AnimationRotation = 0;

        AnimationBone body, eyeLeft, eyeRight;

        Vector2 startingPos;

        public override void Create()
        {
            BaseSpeed = 0.2f; //This monster is a bit slower

            base.Create();
            BoundingBox = new Rectangle(-20, -20, 40, 23);
            SpeedOnHit = new Vector2(2, -1);
            HitPoints = 7;
            Damage = AttackDamage;

            SpriteScale = 0.15f;

            //animation rig
            body = new AnimationBone(this, new Vector2(0, 0));
            eyeLeft = new AnimationBone(body, new Vector2(10, -13));
            eyeRight = new AnimationBone(body, new Vector2(0, -13));

            World.AddObject(body);
            body.SetSprite("Enemy3/Body");
            World.AddObject(eyeLeft);
            eyeLeft.SetSprite("Enemy3/LEye");
            World.AddObject(eyeRight);
            eyeRight.SetSprite("Enemy3/REye");

            State = MonsterState.ChangeState;
            StateTimer = 60;

            Gravity = 0.2f;

            startingPos = Position;

            Depth = -10;
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
            Drawing.DrawRectangle(DrawBoundingBox, Color.Red);
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
            eyeLeft.Destroy();
            eyeRight.Destroy();
        }

        void PlayAnimationIdle()
        {
            eyeLeft.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation + 180);
            eyeRight.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation + 120);
            body.TargetRotation = 0;
        }

        void PlayAnimationWalking()
        {
            body.TargetRotation = 0 + VectorExtensions.LengthDirectionX(3, AnimationRotation);

            eyeLeft.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation);
            eyeRight.TargetRotation = -VectorExtensions.LengthDirectionX(4, AnimationRotation);
        }

        void PlayAnimationInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 2);
            factor += 2;
            factor /= 4;
            
            body.TargetRotation = 10 - factor * 10;
            eyeLeft.TargetRotation = 30 - factor * 30;
            eyeRight.TargetRotation = -eyeLeft.TargetRotation;
        }
    }
}
