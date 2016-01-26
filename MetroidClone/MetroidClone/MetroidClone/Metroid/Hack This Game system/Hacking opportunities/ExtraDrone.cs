using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class ExtraDrone : IHackingOpportunity
    {
        public string Text => "The robot will get an extra drone.";
        public bool IsTimeBased => false;
        public int Time => 0;

        public bool CanUse(World world)
        {
            return world.Player.CollectedScrap > 0;
        }

        public void Use(World world)
        {
            world.AddObject(new Drone(), world.Player.Position);
        }

        public void Stop(World world)
        {
            //Do nothing
        }
    }
}
