using MetroidClone.Engine;
using MetroidClone.Metroid.Monsters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroidClone.Metroid
{
    //A portal which can spawn monsters. Used for the final bossfight.
    class Portal : BoxObject
    {
        public bool Activated = false, Won = false;
        public override bool ShouldDrawGUI => true;

        List<List<Monster>> spawnsEachWave;
        List<Monster> spawns;

        int wave = 0, nextEvent = 0;

        string battleCry; //The text used to announce the arrival of the monsters! 
        bool waveFinished = false;

        float alpha = 1; //The transparancy of the portal.

        int bossHelpersCreated = 0; //The number of extra monsters that have been created for the fight with the boss.

        public override void Create()
        {
            base.Create();
            Depth = 10;
            BoundingBox = new Rectangle(0, 0, (int)(World.TileWidth * (51f / 64f)), World.TileHeight * 2);

            spawns = new List<Monster>();
            nextEvent = 120;

            battleCry = new List<string>() { "You will die!", "We finally got you!", "You are trapped now!", "You have nowhere to go!" }.GetRandomItem();

            //All monster waves
            spawnsEachWave = new List<List<Monster>>();
            spawnsEachWave.Add(new List<Monster>());
            spawnsEachWave.Add(new List<Monster>() { new SlimeMonster(), new SlimeMonster(), new SlimeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new MeleeMonster(), new MeleeMonster(), new MeleeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new SlimeMonster(), new MeleeMonster(), new SlimeMonster(), new MeleeMonster(), new SlimeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new MeleeMonster(), new SlimeMonster(), new ShootingMonster() });
            spawnsEachWave.Add(new List<Monster>() { new ShootingMonster(), new MeleeMonster(), new MeleeMonster(), new SlimeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new ShootingMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new SlimeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new ShootingMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new ShootingMonster(), new MeleeMonster() });
            spawnsEachWave.Add(new List<Monster>() { new ShootingMonster(), new MeleeMonster(), new MeleeMonster(), new MeleeMonster(), new SlimeMonster(), new SlimeMonster(), new MeleeMonster(),
                new MeleeMonster(), new MeleeMonster()});
            spawnsEachWave.Add(new List<Monster>() { new Boss() });
        }

        public override void Update(GameTime gameTime)
        {
            if (Won)
            {
                //Fade out portal.
                if (alpha > 0)
                    alpha -= 0.025f;
            }
            else if (Activated)
            {
                int aliveMonsters = 0;
                Boss boss = null;
                foreach (Monster monster in spawns)
                {
                    if (World.GameObjects.Contains(monster))
                    {
                        aliveMonsters++;

                        if (monster is Boss)
                            boss = monster as Boss;
                    }
                }

                //Check if we should do something and if so do that.
                if (nextEvent > 0)
                    nextEvent--;
                else
                {
                    if (wave == 0)
                        wave = 1;

                    if (spawnsEachWave[wave].Count > 0)
                    {
                        waveFinished = false;

                        //Spawn a monster, give it unlimited looking distance, and add it to the world.
                        Monster newMonster = spawnsEachWave[wave].GetRandomItem();
                        newMonster.HasUnlimitedLookingDistance = true;
                        spawnsEachWave[wave].Remove(newMonster);
                        World.AddObject(newMonster, new Vector2(Position.X + BoundingBox.Width / 2, Position.Y + BoundingBox.Height / 2 + 20));
                        spawns.Add(newMonster);
                        nextEvent = 30;
                    }
                    else
                    {
                        //Check if the wave is finished.
                        if (aliveMonsters == 0)
                        {
                            if (wave < spawnsEachWave.Count - 1)
                            {
                                wave++;
                                waveFinished = true;

                                nextEvent = 150; //Give the player a short time to breathe.
                            }
                            else
                            {
                                Won = true;

                                //Open boss doors.
                                foreach (BossDoor door in World.GameObjects.OfType<BossDoor>())
                                {
                                    door.Activated = true;
                                }
                            }
                        }
                    }
                }

                //Possibly spawn extra monsters to help the boss.
                if (boss != null)
                {
                    if (boss.HitPoints < 80 && bossHelpersCreated < 1)
                    {
                        bossHelpersCreated = 1;
                        spawnsEachWave[wave] = new List<Monster>() { new SlimeMonster(), new SlimeMonster() };
                    }
                    else if (boss.HitPoints < 50 && bossHelpersCreated < 2)
                    {
                        bossHelpersCreated = 2;
                        spawnsEachWave[wave] = new List<Monster>() { new MeleeMonster(), new MeleeMonster(), new MeleeMonster() };
                    }
                    else if (boss.HitPoints < 20 && bossHelpersCreated < 3)
                    {
                        bossHelpersCreated = 3;
                        spawnsEachWave[wave] = new List<Monster>() { new ShootingMonster() };
                    }
                }
            }
        }

        public override void Draw()
        {
            if (alpha > 0)
                Drawing.DrawSprite("portal", DrawPosition, size: ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height), color: Color.White * alpha);
        }

        public override void DrawGUI()
        {
            string text = "";
            if (Won)
                text = "You destroyed the portal!";
            else if (wave == 0)
                text = battleCry;
            else
            {
                if (waveFinished)
                    text = "Wave " + (wave - 1) + " finished!";
                else
                    text = "Wave " + wave + " of " + (spawnsEachWave.Count - 1);
            }

            //Draw information about the bossfight.
            if (Activated)
            {
                Drawing.DrawText("fontboss", text, new Vector2(Drawing.GUISize.X / 2, 0), Color.Black, alignment: Engine.Asset.Font.Alignment.TopCenter);
                Drawing.DrawText("fontboss", text, new Vector2(Drawing.GUISize.X / 2 + 1, 1), Color.White, alignment: Engine.Asset.Font.Alignment.TopCenter);
            }
        }
    }
}
