using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class DroneRangeUpgrade : UpgradePickup
    {
        public override string Text => "Drones now have a bigger range!";

        public override void DoUpgrade()
        {
            World.Player.HasDroneAttentionRadiusUpgrade = true;
        }
    }
}
