using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class FastMonsters : IHackingOpportunity
    {
        public string Text => "Monsters move much faster for a while.";
        public bool IsTimeBased => true;
        public int Time => 12;

        public bool CanUse(World world)
        {
            //Once the gun has been collected, there is an ok chance that there are monsters or will be monsters soon.
            return world.Player.UnlockedWeapons.Contains(Weapon.Gun);
        }

        public void Use(World world)
        {
            Monster.SpeedModifier *= 2f;
        }

        public void Stop(World world)
        {
            Monster.SpeedModifier /= 2f;
        }
    }

    class SlowerMonsters : IHackingOpportunity
    {
        public string Text => "Monsters move much slower for a while.";
        public bool IsTimeBased => true;
        public int Time => 12;

        public bool CanUse(World world)
        {
            //Once the gun has been collected, there is an ok chance that there are monsters or will be monsters soon.
            return world.Player.UnlockedWeapons.Contains(Weapon.Gun);
        }

        public void Use(World world)
        {
            Monster.SpeedModifier /= 2;
        }

        public void Stop(World world)
        {
            Monster.SpeedModifier *= 2;
        }
    }
}
