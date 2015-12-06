using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    public enum SideType { Any, Wall, Exit };

    class LevelBlockRequirements
    {
        public SideType LeftSideType, RightSideType, TopSideType, BottomSideType;

        public LevelBlockRequirements()
        {
            LeftSideType = SideType.Any;
            RightSideType = SideType.Any;
            TopSideType = SideType.Any;
            BottomSideType = SideType.Any;
        }
    }
}
