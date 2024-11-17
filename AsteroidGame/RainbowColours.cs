using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace Asteroid_Game
{


    public class RainbowColours
    {
        public List<Color> listColours = new List<Color>();
        public int index = 0;

        public RainbowColours()
        {
            listColours.Add(new Color(255, 0, 0));
            listColours.Add(new Color(255, 255, 0));
            listColours.Add(new Color(0, 255, 0));
            listColours.Add(new Color(0, 255, 255));
            listColours.Add(new Color(0, 0, 255));
            listColours.Add(new Color(255, 0, 255));
            listColours.Add(new Color(255, 0, 0));
        }

        public void IncIndex()
        {
            ++index;
            index %= listColours.Count;
        }
        public void DecIndex()
        {
            index += listColours.Count -1;
            index %= listColours.Count;
        }

        public Color GetNextColour()
        {
            this.IncIndex();
            return GetCurrent();
        }
        public Color GetPrevColour()
        {
            this.DecIndex();
            return GetCurrent();
        }

        public Color GetCurrent()
        {
            return listColours[index];
        }


        public List<Color> GetNextColourSet()
        {
            return new List<Color>(new Color[] { GetPrevColour(), GetNextColour(), GetCurrent(), GetNextColour()});
        }

        public static Color MergeColours(Color c1, Color c2)
        {
            int r = (int)((c1.R + c2.R) * 0.5f);
            int g = (int)((c1.G + c2.G) * 0.5f);
            int b = (int)((c1.B + c2.B) * 0.5f);
            int a = (int)((c1.A + c2.A) * 0.5f);
            return new Color(r, g, b, a);
        }
    }
}
