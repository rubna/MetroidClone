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
        public int Damage = 10;
        protected int ScoreOnKill = 1;
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
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score += ScoreOnKill;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }
        public override void Destroy()
        {
            World.Tutorial.MonsterKilled = true;
            // When a monster is destroyed, you have a chance that a healthpack, rocket ammo, scrap metal or nothing will drop
            float ammoChance = (1 - (World.Player.RocketAmmo / World.Player.MaximumRocketAmmo)) * 40;
            float scrapChance = 40;
            float healthChance = (1f - ((float)World.Player.HitPoints / (float)World.Player.MaximumHitPoints)) * 40;
            float randomLoot = World.Random.Next(101);
            if (randomLoot <= healthChance)
                World.AddObject(new HealthDrop(Damage), Position);
            else if (randomLoot <= ammoChance + healthChance)
                World.AddObject(new RocketAmmo(), Position);
            else if (randomLoot <= scrapChance + ammoChance + healthChance)
                World.AddObject(new Scrap(), Position);
            base.Destroy();
        }
    }
}
