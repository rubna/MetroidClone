using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MetroidClone.Engine
{
    class GameObject
    {
        public Vector2 Position = Vector2.Zero;

        public float Depth = 0;

        public World World;
        public DrawWrapper Drawing;
        public AssetManager Assets;
        protected InputHelper Input;

        //Information about the sprite. For objects without a sprite, CurrentSprite is null.
        protected Sprite CurrentSprite;
        protected float CurrentImage;
        protected bool AnimationIsRepeating;
        protected float AnimationSpeed;

        //Event handlers
        public event EventHandler AnimationFinished;

        public virtual void Create()
        {
            Input = InputHelper.Instance;

            CurrentSprite = null;
            CurrentImage = 0;
            AnimationIsRepeating = false;
            AnimationSpeed = 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateAnimation();
        }

        protected void UpdateAnimation()
        {
            if (CurrentSprite?.ImageNumber > 1) //If the current sprite isn't null, check if it should be animated.
            {
                CurrentImage += AnimationSpeed;
                Console.Write(true);
            }
        }

        public virtual void Draw()
        {
            if (CurrentSprite != null)
                 Drawing.DrawSprite(CurrentSprite, Position, (int) CurrentImage); //Draw the current image of the sprite.
        }

        //Plays an animation. 
        protected void PlayAnimation(string sprite, float startImage = 0, bool repeat = true, float speed = 1)
        {
            CurrentSprite = Assets.GetSprite(sprite);
            CurrentImage = startImage;
            AnimationIsRepeating = repeat;
            AnimationSpeed = speed;
        }

        //Methods to call when events occur
        protected virtual void OnAnimationFinished(EventArgs e)
        {
            if (AnimationFinished != null)
                AnimationFinished(this, e);
        }
    }
}
