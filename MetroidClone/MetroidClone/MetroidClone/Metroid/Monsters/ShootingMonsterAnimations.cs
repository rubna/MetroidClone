using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid.Monsters
{
    partial class ShootingMonster : Monster
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

        void PlayAnimationArmsIdle()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowLeft.TargetRotation = -90 - VectorExtensions.LengthDirectionX(1, AnimationRotation);

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
            elbowLeft.TargetRotation = -50 - factor * 100;

            shoulderRight.TargetRotation = -110 + direction + factor * 80;//120
            elbowRight.TargetRotation = -80 - factor * 80;
            gun.TargetRotation = 110 + 80;
            head.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.5f;
            body.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.2f;
        }
    }
}
