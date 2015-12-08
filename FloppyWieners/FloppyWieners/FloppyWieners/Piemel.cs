using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FloppyWieners
{
    class Piemel : GameObject
    {
        List<GameObject> stukkies;
        int aantalStukkies = 20;
        float distance = 10;
        float springFactor = 0.1f;
        float excitement = 0f;

        public override void Create()
        {
            base.Create();
            stukkies = new List<GameObject>();
            for (int v = 0; v < aantalStukkies; v++)
            {
                GameObject stukkie = new GameObject() { Position = Position + new Vector2(0, distance * v), Gravity = 0.1f };
                World.AddObject(stukkie);
                stukkies.Add(stukkie);
            }
        }

        public override void Update()
        {
            base.Update();
            //Position = Input.MousePosition();
            if (Input.KeyboardCheckDown(Keys.Space))
                excitement += (1 - excitement) * 0.001f;
            else
                excitement = 0;
            distance = 3 + excitement * 7;

            stukkies[0].Position = Position;
            for (int i = 1; i < aantalStukkies; i++)
            {
                GameObject stuk = stukkies[i];
                stuk.Speed += CalculateSpring(stuk.Position, stukkies[0].Position + new Vector2(distance * i, -45).ToCartesian(), excitement, 0);

                for (int j = Math.Max(0, i - 100); j< Math.Min(aantalStukkies, i + 100); j++)
                {
                    if (i == j)
                        continue;
                    int d = Math.Abs(i - j);
                    stuk.Speed += CalculateSpring(stuk.Position, stukkies[j].Position, springFactor, distance * d);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            List<Vector2> verts = new List<Vector2>();
            foreach (GameObject vert in stukkies)
            {
                verts.Add(vert.Position);
            }
            Drawing.DrawPrimitive(PrimitiveType.LineStrip, verts, Color.Red);
        }

        Vector2 CalculateSpring(Vector2 from, Vector2 to, float springAmount, float baseDistance)
        {
            Vector2 dVector = to - from;
            dVector = dVector.ToPolar();
            dVector.X -= baseDistance;
            return dVector.ToCartesian() * springAmount;
        }
    }
}
