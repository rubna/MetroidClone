using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class GunDoor : Door
    {
        protected override string closedSprite { get { return "doorspriteclosedgreen"; } }
        protected override string standardSprite { get { return "doorspriteopen"; } }
    }
}
