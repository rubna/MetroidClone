using MetroidClone.Engine;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        protected MonsterState State;
        protected enum MonsterState { Moving, Attacking, Jumping, PatrollingLeft, PatrollingRight, ChangeState }

        public int HitPoints = 1;
        public int Damage = 10;
        protected Vector2 SpeedOnHit = Vector2.Zero;

        public Monster()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                {
                    Hurt(Math.Sign(Position.X - attack.Position.X), true);
                    attack.Destroy();
                }
            }
        }

        void Hurt(int xDirection, bool hitByPlayer)
        {
            // if am attack from the player hits
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score += 20;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }

        public override void Destroy()
        {
            int randomLoot;
            base.Destroy();
            // When a monster is destroyed, you have a chance that a rocket or a health pack will drop
            if (World.Player.UnlockedWeapons.Contains(Weapon.Rocket))
            {
                randomLoot = World.Random.Next(100);
                if (randomLoot < 5 || randomLoot < 15 && World.Player.RocketAmmo < 3)
                    World.AddObject(new RocketAmmo(), Position.X, Position.Y);
                if (randomLoot > 14 && randomLoot < 20 || randomLoot > 14 && randomLoot < 30 && World.Player.HitPoints < 25)
                    World.AddObject(new HealthDrop(), Position.X, Position.Y);
            }
            else
            {
                randomLoot = World.Random.Next(100);
                if (randomLoot < 5 || randomLoot < 15 && World.Player.HitPoints < 25)
                    World.AddObject(new HealthDrop(), Position.X, Position.Y);
            }
        }
    }
}
