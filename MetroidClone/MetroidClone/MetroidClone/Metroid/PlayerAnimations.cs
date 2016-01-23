using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    partial class Player : PhysicsObject
    {
        void PlayAnimationWalking()
        {
            PlayAnimationLegsWalking();
            if (CurrentWeapon == Weapon.Nothing)
                PlayAnimationArmsLooseWalking();
            else
            if (CurrentWeapon == Weapon.Gun)
            {
                if (shotAnimationTimer>0)
                    PlayAnimationArmsGunShooting(shootDirection);
                else
                    PlayAnimationArmsGunWalking();
            }
            else
            if (CurrentWeapon == Weapon.Rocket)
            {
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsLauncherShooting(shootDirection);
                else
                    PlayAnimationArmsLauncherWalking();
            }
        }
        void PlayAnimationIdle()
        {
            PlayAnimationLegsIdle();
            if (CurrentWeapon == Weapon.Nothing)
                PlayAnimationArmsLooseIdle();
            else
            if (CurrentWeapon == Weapon.Gun)
            {
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsGunShooting(shootDirection);
                else
                    PlayAnimationArmsGunIdle();
            }
            else
            if (CurrentWeapon == Weapon.Rocket)
            {
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsLauncherShooting(shootDirection);
                else
                    PlayAnimationArmsLauncherIdle();
            }
        }

        void PlayAnimationDuck()
        {
            PlayAnimationLegsDuck();
            if (CurrentWeapon == Weapon.Nothing)
                PlayAnimationArmsLooseDuck();
            else
            {
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsGunShooting(shootDirection);
                else
                    PlayAnimationArmsGunDuck();
            }
        }

        void PlayAnimationInAir()
        {
            PlayAnimationLegsInAir();
            if (CurrentWeapon == Weapon.Nothing)
                PlayAnimationArmsLooseInAir();
            else
            {
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsGunShooting(shootDirection);
                else
                    PlayAnimationArmsGunInAir();
            }
        }

        void PlayAnimationArmsGunShooting(float direction)
        {
            Vector2 cart = new Vector2(1, direction).ToCartesian();
            if (FlipX)
                cart.X *= -1;
            direction = cart.Angle();

            float factor = MathHelper.Clamp(shotAnimationTimer, 0, 1);
            //factor *= 2;

            shoulderLeft.TargetRotation = 30 + VectorExtensions.AngleDifference(0, direction) * 1.5f + factor * 30;
            elbowLeft.TargetRotation = -50 - factor * 50;

            shoulderRight.TargetRotation = -110 + direction + factor * 100;//120
            elbowRight.TargetRotation = -80 - factor * 100;
            gun.TargetRotation = 110 + 80;
            head.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.5f;
            body.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.2f;
        }

        void PlayAnimationArmsLauncherShooting(float direction)
        {
            Vector2 cart = new Vector2(1, direction).ToCartesian();
            if (FlipX)
                cart.X *= -1;
            direction = cart.Angle();

            float factor = MathHelper.Clamp(shotAnimationTimer, 0, 1);
            //factor *= 2;

            shoulderLeft.TargetRotation = 30 + VectorExtensions.AngleDifference(0, direction) * 1.5f + factor * 30;
            elbowLeft.TargetRotation = -50 - factor * 50;

            shoulderRight.TargetRotation = -110 + direction + factor * 80;//120
            elbowRight.TargetRotation = -80 - factor * 80;
            launcher.TargetRotation = 110 + 80;
            head.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.5f;
            body.TargetRotation = VectorExtensions.AngleDifference(0, direction) * 0.2f;
        }

        void PlayAnimationArmsLooseWalking()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(50, AnimationRotation + 180);
            elbowLeft.TargetRotation = -40 - VectorExtensions.LengthDirectionX(40, AnimationRotation);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(60, AnimationRotation);
            elbowRight.TargetRotation = -40 - VectorExtensions.LengthDirectionX(40, AnimationRotation + 180);
        }

        void PlayAnimationArmsGunWalking()
        {
            shoulderLeft.TargetRotation = 85 + VectorExtensions.LengthDirectionX(30, AnimationRotation + 180);
            elbowLeft.TargetRotation = -80 - VectorExtensions.LengthDirectionX(10, AnimationRotation + 180);

            shoulderRight.TargetRotation = -50 + VectorExtensions.LengthDirectionX(30, AnimationRotation + 180);
            elbowRight.TargetRotation = -90 - VectorExtensions.LengthDirectionX(5, AnimationRotation + 180);
            gun.TargetRotation = 140 + VectorExtensions.LengthDirectionX(20, AnimationRotation);
        }

        void PlayAnimationArmsLauncherWalking()
        {
            shoulderLeft.TargetRotation = 85 + VectorExtensions.LengthDirectionX(30, AnimationRotation + 180);
            elbowLeft.TargetRotation = -80 - VectorExtensions.LengthDirectionX(10, AnimationRotation + 180);

            shoulderRight.TargetRotation = -50 + VectorExtensions.LengthDirectionX(30, AnimationRotation + 180);
            elbowRight.TargetRotation = -120 - VectorExtensions.LengthDirectionX(5, AnimationRotation + 180);
            launcher.TargetRotation = 180 + VectorExtensions.LengthDirectionX(20, AnimationRotation);
        }

        void PlayAnimationLegsWalking()
        {
            body.Offset = body.OriginalOffset + new Vector2(0.5f, 1) * new Vector2(4, AnimationRotation * 2).ToCartesian();
            hipLeft.Offset = hipLeft.OriginalOffset + new Vector2(0.5f, 1) * new Vector2(2, AnimationRotation).ToCartesian();
            hipRight.Offset = hipRight.OriginalOffset + new Vector2(0.5f, 1) * new Vector2(2, AnimationRotation + 180).ToCartesian();

            body.TargetRotation = VectorExtensions.LengthDirectionX(8, AnimationRotation + 180);
            head.TargetRotation = VectorExtensions.LengthDirectionX(4, AnimationRotation);
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

            antennaLeft1.TargetRotation = 15 + VectorExtensions.LengthDirectionY(10, AnimationRotation * 2 + 50);
            antennaRight1.TargetRotation = 12 + VectorExtensions.LengthDirectionY(12, AnimationRotation * 2);

            antennaLeft2.TargetRotation = 70 + VectorExtensions.LengthDirectionX(20, AnimationRotation * 2 + 60);
            antennaRight2.TargetRotation = 60 + VectorExtensions.LengthDirectionX(16, AnimationRotation * 2);

        }

        void PlayAnimationLegsIdle()
        {
            body.Offset = body.OriginalOffset + new Vector2(0, 1) + new Vector2(0, 1) * new Vector2(0.5f, AnimationRotation).ToCartesian();
            hipLeft.Offset = hipLeft.OriginalOffset;
            hipRight.Offset = hipRight.OriginalOffset;

            body.TargetRotation = 1;
            head.TargetRotation = -1;
            head.Offset = head.OriginalOffset;
            hipLeft.TargetRotation = -7 + VectorExtensions.LengthDirectionY(12, AnimationRotation + 180);
            kneeLeft.TargetRotation = 15 + VectorExtensions.LengthDirectionY(25, AnimationRotation);

            hipRight.TargetRotation = -30 + VectorExtensions.LengthDirectionY(11, AnimationRotation + 180);
            kneeRight.TargetRotation = 30 + VectorExtensions.LengthDirectionY(22, AnimationRotation);


            antennaLeft1.TargetRotation = 15 + VectorExtensions.LengthDirectionX(1, AnimationRotation + 50);
            antennaRight1.TargetRotation = 12 + VectorExtensions.LengthDirectionX(1, AnimationRotation);

            antennaLeft2.TargetRotation = 70 + VectorExtensions.LengthDirectionX(4, AnimationRotation + 60);
            antennaRight2.TargetRotation = 60 + VectorExtensions.LengthDirectionX(4, AnimationRotation);
        }

        void PlayAnimationArmsLooseIdle()
        {
            shoulderLeft.TargetRotation = 80;
            elbowLeft.TargetRotation = -20;

            shoulderRight.TargetRotation = -45;
            elbowRight.TargetRotation = -20;
        }

        void PlayAnimationArmsGunIdle()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowLeft.TargetRotation = -90 - VectorExtensions.LengthDirectionX(1, AnimationRotation);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowRight.TargetRotation = -100 - VectorExtensions.LengthDirectionX(1, AnimationRotation);
            gun.TargetRotation = 180 + VectorExtensions.LengthDirectionX(3, AnimationRotation);
        }


        void PlayAnimationArmsLauncherIdle()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowLeft.TargetRotation = -90 - VectorExtensions.LengthDirectionX(1, AnimationRotation);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowRight.TargetRotation = -100 - VectorExtensions.LengthDirectionX(1, AnimationRotation);
            launcher.TargetRotation = 180 + VectorExtensions.LengthDirectionX(3, AnimationRotation);
        }

        void PlayAnimationLegsInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 2);
            factor += 2;
            factor /= 4;

            body.Offset = body.OriginalOffset;
            hipLeft.Offset = hipLeft.OriginalOffset;
            hipRight.Offset = hipRight.OriginalOffset;

            body.TargetRotation = -10 + factor * 15;
            head.TargetRotation = -10 + factor * 20;
            head.Offset = head.OriginalOffset;

            hipLeft.TargetRotation = 30 - factor * 50;
            kneeLeft.TargetRotation = 0 + factor * 10;

            hipRight.TargetRotation = -90 + factor * 45;
            kneeRight.TargetRotation = 90 - factor * 85;

            antennaLeft1.TargetRotation = 30 - factor * 15;
            antennaRight1.TargetRotation = 26 - factor * 15;

            antennaLeft2.TargetRotation = 95 - factor * 80;
            antennaRight2.TargetRotation = 100 - factor * 75;
        }

        void PlayAnimationArmsLooseInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 3);
            factor += 2;
            factor /= 5;

            shoulderLeft.TargetRotation = -20 + factor * 30;
            elbowLeft.TargetRotation = -70 + factor * 90;

            shoulderRight.TargetRotation = -45 + factor * 80;
            elbowRight.TargetRotation = -10 - factor * 20;
        }

        void PlayAnimationArmsGunInAir()
        {
            float factor = MathHelper.Clamp(Speed.Y, -2, 3);
            factor += 2;
            factor /= 5;

            shoulderLeft.TargetRotation = 60 - factor * 30;
            elbowLeft.TargetRotation = 20 - factor * 95;

            shoulderRight.TargetRotation = -90 - factor * 50;
            elbowRight.TargetRotation = -50 - factor * 40;
            gun.TargetRotation = 140 + factor * 40;
        }

        void PlayAnimationLegsDuck()
        {
            body.Offset = body.OriginalOffset + new Vector2(0, 8) + new Vector2(0, 1) * new Vector2(0.5f, AnimationRotation).ToCartesian();
            body.TargetRotation = 20;
            head.TargetRotation = -15;
            head.Offset = head.OriginalOffset + new Vector2(0, 3);
            hipLeft.Offset = hipLeft.OriginalOffset;
            hipRight.Offset = hipRight.OriginalOffset;

            hipLeft.TargetRotation = -80 + VectorExtensions.LengthDirectionY(6, AnimationRotation + 180);
            kneeLeft.TargetRotation = 100 + VectorExtensions.LengthDirectionY(12, AnimationRotation);

            hipRight.TargetRotation = -120 + VectorExtensions.LengthDirectionY(5, AnimationRotation + 180);
            kneeRight.TargetRotation = 100 + VectorExtensions.LengthDirectionY(10, AnimationRotation);
        }

        void PlayAnimationArmsLooseDuck()
        {
            shoulderLeft.TargetRotation = 60;
            elbowLeft.TargetRotation = -50;

            shoulderRight.TargetRotation = -45;
            elbowRight.TargetRotation = -20;
        }

        void PlayAnimationArmsGunDuck()
        {
            shoulderLeft.TargetRotation = 90 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowLeft.TargetRotation = -90 - VectorExtensions.LengthDirectionX(1, AnimationRotation);

            shoulderRight.TargetRotation = -80 + VectorExtensions.LengthDirectionX(3, AnimationRotation + 180);
            elbowRight.TargetRotation = -100 - VectorExtensions.LengthDirectionX(1, AnimationRotation);
            gun.TargetRotation = 180 + VectorExtensions.LengthDirectionX(3, AnimationRotation);
        }
    }
}
