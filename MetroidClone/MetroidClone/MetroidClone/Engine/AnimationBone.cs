using Microsoft.Xna.Framework;
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
        public Vector2 TargetPosition
        {
            get
            {
                Vector2 pos = (Offset * Parent.ImageScaling).ToPolar();
                pos.Y += Parent.ImageRotation;
                if (Parent is AnimationBone)
                {
                    AnimationBone par = Parent as AnimationBone;
                    return par.TargetPosition + pos.ToCartesian();
                }
                else
                    return Parent.Position + pos.ToCartesian();
            }
        }
        public float DepthOffset = -1;

        public AnimationBone(PhysicsObject parent)
        {
            Parent = parent;
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
            Position = TargetPosition;
            //Speed = (TargetPosition - Position);
            Depth = Parent.Depth-20 + DepthOffset;
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawLine(TargetPosition, Parent.Position, 2, Color.Pink);
            Drawing.DrawCircle(Position, 3, Color.Purple);
        }
    }
}
