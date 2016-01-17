using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid.Monsters
{
    class TestMonster : Monster
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 10;
            Damage = 5;
            ScoreOnKill = 20;
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Red);
        }
    }
}
