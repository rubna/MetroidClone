using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    //With this effect, the player will get extra hitpoints.
    class MoreHP : IHackingOpportunity
    {
        public string Text => "The robot will heal a bit.";
        public bool IsTimeBased => false;
        public int Time => 0;

        public bool CanUse(World world)
        {
            return world.Player.HitPoints < world.Player.MaxHitPoints;
        }

        public void Use(World world)
        {
            world.Player.HitPoints = Math.Min(world.Player.MaxHitPoints, world.Player.HitPoints + 30);
        }

        public void Stop(World world)
        {
            //Do nothing
        }
    }

    //With this effect, the player will get extra rockets.
    class MoreRockets : IHackingOpportunity
    {
        public string Text => "The robot will get some extra rockets.";
        public bool IsTimeBased => false;
        public int Time => 0;

        public bool CanUse(World world)
        {
            return world.Player.UnlockedWeapons.Contains(Weapon.Rocket);
        }

        public void Use(World world)
        {
            world.Player.RocketAmmo += 3;
        }

        public void Stop(World world)
        {
            //Do nothing
        }
    }
}
