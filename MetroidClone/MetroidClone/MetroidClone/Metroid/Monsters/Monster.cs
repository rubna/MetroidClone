using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        public int HitPoints = 1;
        public int Damage = 1;
        protected int ScoreOnKill = 1;
        protected Vector2 SpeedOnHit = Vector2.Zero;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                {
                    attack.Destroy();
                    Hurt(Math.Sign(Position.X - attack.Position.X), true);
                }
                }
        }

        void Hurt(int xDirection, bool hitByPlayer)
        {
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score = World.Player.Score + ScoreOnKill;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }
    }
}
