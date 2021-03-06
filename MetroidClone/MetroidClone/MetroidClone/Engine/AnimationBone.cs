﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class AnimationBone : PhysicsObject
    {
        public PhysicsObject Parent { get; private set; } = null;
        public Vector2 Offset = Vector2.Zero;
        public Vector2 OriginalOffset;
        public Vector2 TargetPosition
        {
            get
            {
                Vector2 pos = (Offset * Parent.ImageScaling * new Vector2(Parent.GetFlip, 1)).ToPolar();

                if (Parent is AnimationBone)
                {
                    AnimationBone par = Parent as AnimationBone;
                    pos.Y += par.GlobalRotation * Parent.GetFlip;
                    return par.TargetPosition + pos.ToCartesian();
                }
                else
                    return Parent.Position + pos.ToCartesian();
            }
        }

        public new float SpriteScale = 0;// = 0.08f;
        public float TargetRotation = 0;
        public float RotationLerpFactor = 0.3f;
        public float PositionLerpFactor = 0.9f;

        public float GlobalRotation
        {
            get
            {
                if (Parent is AnimationBone)
                {
                    var par = Parent as AnimationBone;
                    return ImageRotation + par.GlobalRotation;
                }
                else
                    return ImageRotation + Parent.ImageRotation;
            }
        }
        public bool GlobalVisible
        {
            get
            {
                if (Parent is AnimationBone)
                {
                    var par = Parent as AnimationBone;
                    return par.GlobalVisible;
                }
                else
                    return Parent.Visible;
            }
        }
        public float GlobalSpriteScale
        {
            get
            {
                if (SpriteScale == 0)
                {
                    if (Parent is AnimationBone)
                    {
                        var par = Parent as AnimationBone;
                        return par.GlobalSpriteScale;
                    }
                    else
                        return Parent.SpriteScale;
                }
                else
                    return SpriteScale;
            }
        }

        public float RotationOffset = 0;

        public Vector2 TargetDrawPosition
        {
            get
            {
                return Position - World.Camera;
            }
        }
        public float DepthOffset = -1;

        public AnimationBone(PhysicsObject parent, Vector2 offset, float rotationOffset = 0)
        {
            Parent = parent;
            Offset = offset;
            RotationOffset = rotationOffset;
            OriginalOffset = offset;
            Gravity = 0;
        }

        public override void Create()
        {
            base.Create();
            Position = TargetPosition;
            CollideWithWalls = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Speed = (TargetPosition - Position) * PositionLerpFactor;

            if (!World.PointOutOfView(Position))
            {
                FlipX = Parent.FlipX;
                ImageRotation += VectorExtensions.AngleDifference(ImageRotation, TargetRotation) * RotationLerpFactor;
                Depth = Parent.Depth + DepthOffset;
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (CurrentSprite != null && Visible && GlobalVisible)
                Drawing.DrawSprite(CurrentSprite, DrawPosition, 0, CurrentSprite.Size * GlobalSpriteScale * new Vector2(GetFlip, 1), null, MathHelper.ToRadians(GlobalRotation) * GetFlip);// ImageScaling, Color.White, ImageRotation);
        }
    }
}
