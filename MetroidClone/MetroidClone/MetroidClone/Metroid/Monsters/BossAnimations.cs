using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid.Monsters
{
    partial class Boss : Monster
    {
        void PlayAnimationLegsIdle()
        {
            body.Offset = body.OriginalOffset + new Vector2(0, 1) + new Vector2(0, 1) * new Vector2(0.5f, AnimationRotation).ToCartesian();
            hipLeft.Offset = hipLeft.OriginalOffset;
            hipRight.Offset = hipRight.OriginalOffset;

            body.TargetRotation = 1;
            head.TargetRotation = -1;
            head.Offset = head.OriginalOffset;
            hipLeft.TargetRotation = -20 + VectorExtensions.LengthDirectionY(10, AnimationRotation + 180);
            kneeLeft.TargetRotation = 15 + VectorExtensions.LengthDirectionY(20, AnimationRotation);

            hipRight.TargetRotation = 0 + VectorExtensions.LengthDirectionY(11, AnimationRotation + 180);
            kneeRight.TargetRotation = 10 + VectorExtensions.LengthDirectionY(22, AnimationRotation);
        }

        void PlayAnimationLegsWalking()
        {
            body.Offset = body.OriginalOffset - new Vector2(0, -1) + new Vector2(0.5f, 1) * new Vector2(4, AnimationRotation * 2).ToCartesian();
            hipLeft.Offset = hipLeft.OriginalOffset + new Vector2(0.5f, 1) * new Vector2(2, AnimationRotation).ToCartesian();
            hipRight.Offset = hipRight.OriginalOffset + new Vector2(0.5f, 1) * new Vector2(2, AnimationRotation + 180).ToCartesian();

            body.TargetRotation = 6 + VectorExtensions.LengthDirectionX(2, AnimationRotation * 2);
            head.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation + 180);
            head.Offset = head.OriginalOffset;
            hipLeft.TargetRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation + 180);
            hipRight.TargetRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation);

            float kneeRot = VectorExtensions.LengthDirectionX(90, AnimationRotation - 50 + 180);
            if (VectorExtensions.AngleDifference(kneeRot, 0) > 0)
                kneeRot = 0;
            kneeLeft.TargetRotation = kneeRot;

            kneeRot = VectorExtensions.LengthDirectionX(90, AnimationRotation - 50);
            if (VectorExtensions.AngleDifference(kneeRot, 0) > 0)
                kneeRot = 0;
            kneeRight.TargetRotation = kneeRot;
        }

        void PlayAnimationArmsWalking()
        {
            shoulderLeft.TargetRotation = 85 + VectorExtensions.LengthDirectionX(20, AnimationRotation * 2 + 180);
            elbowLeft.TargetRotation = -110 - VectorExtensions.LengthDirectionX(30, AnimationRotation * 2 + 180);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(20, AnimationRotation * 2 + 180);
            elbowRight.TargetRotation = -110 - VectorExtensions.LengthDirectionX(30, AnimationRotation * 2 + 180);
            gun.TargetRotation = 180 + VectorExtensions.LengthDirectionX(-20, AnimationRotation * 2);
        }

        void PlayAnimationArmsIdle()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowLeft.TargetRotation = -100 - VectorExtensions.LengthDirectionX(1, AnimationRotation);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowRight.TargetRotation = -100 - VectorExtensions.LengthDirectionX(1, AnimationRotation);
            gun.TargetRotation = 180 + VectorExtensions.LengthDirectionX(3, AnimationRotation);
        }

        void PlayAnimationArmsShooting(float direction)
        {
            Vector2 cart = new Vector2(1, direction).ToCartesian();
            if (FlipX)
                cart.X *= -1;
            direction = cart.Angle();

            float factor = MathHelper.Clamp(shotAnimationTimer, 0, 1);

            shoulderLeft.TargetRotation = 30 + VectorExtensions.AngleDifference(0, direction) +  factor * 100;
            elbowLeft.TargetRotation = -60 - factor * 100;

            shoulderRight.TargetRotation = -110 + direction + factor * 80;//120
            elbowRight.TargetRotation = -80 - factor * 80;
            gun.TargetRotation = 110 + 80;
            head.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.5f;
            body.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.2f;
        }


        void PlayAnimationLegsInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 2);
            factor += 2;
            factor /= 4;

            body.Offset = body.OriginalOffset;
            hipRight.Offset = hipRight.OriginalOffset;
            hipLeft.Offset = hipLeft.OriginalOffset;

            body.TargetRotation = -10 + factor * 15;
            head.TargetRotation = -10 + factor * 20;
            head.Offset = head.OriginalOffset;

            hipRight.TargetRotation = 50 - factor * 70;
            kneeRight.TargetRotation = 0 + factor * 10;

            hipLeft.TargetRotation = -90 + factor * 45;
            kneeLeft.TargetRotation = 100 - factor * 95; ;
        }
    }
}
