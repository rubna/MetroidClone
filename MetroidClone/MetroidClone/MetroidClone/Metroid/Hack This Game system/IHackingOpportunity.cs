using MetroidClone.Engine;

namespace MetroidClone.Metroid
{
    interface IHackingOpportunity
    {
        string Text { get; }
        bool CanUse(World world);
        void Use(World world);
        void Stop(World world);
        bool IsTimeBased { get; }
        int Time { get; }
    }
}
