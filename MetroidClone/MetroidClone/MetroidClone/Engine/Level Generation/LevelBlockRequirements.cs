namespace MetroidClone.Engine
{
    public enum SideType { Any, Wall, Exit };

    class LevelBlockRequirements
    {
        public SideType LeftSideType, RightSideType, TopSideType, BottomSideType;
        public string Group;
        public string Theme;

        public LevelBlockRequirements()
        {
            LeftSideType = SideType.Any;
            RightSideType = SideType.Any;
            TopSideType = SideType.Any;
            BottomSideType = SideType.Any;

            Group = "";
            Theme = "";
        }

        public LevelBlockRequirements(SideType sideType)
        {
            LeftSideType = sideType;
            RightSideType = sideType;
            TopSideType = sideType;
            BottomSideType = sideType;

            Group = "";
            Theme = "";
        }

        public bool IsOnlyWalls()
        {
            return LeftSideType == SideType.Wall && RightSideType == SideType.Wall && TopSideType == SideType.Wall && BottomSideType == SideType.Wall;
        }
    }
}
