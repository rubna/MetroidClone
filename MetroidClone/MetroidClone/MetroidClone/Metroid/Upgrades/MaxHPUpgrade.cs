using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class MaxHPUpgrade : UpgradePickup
    {
        public override string Text => "Your maximum HP has been increased!";

        public override void DoUpgrade()
        {
            World.Player.HitPoints += 20;
            World.Player.MaxHitPoints += 20;
        }
    }

    class HugeMaxHPUpgrade : UpgradePickup
    {
        public override string Text => "Your maximum HP has been increased by a large amount!";

        public override void DoUpgrade()
        {
            World.Player.HitPoints += 50;
            World.Player.MaxHitPoints += 50;
        }
    }
}
