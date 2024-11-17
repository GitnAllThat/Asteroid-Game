using System.Collections.Generic;


namespace Asteroid_Game
{
    public class DyingRainbowLine
    {
        public static float shortenBy = 0.4f;
        public static float minLength = 0.01f;

        
        public List<RainbowLine> rainbowLine = new List<RainbowLine>();
        public bool fromFront;
        public float speedModifier = 1;

        public DyingRainbowLine(List<RainbowLine> rLine, bool fromFront, float speedModifier)
        {
            this.rainbowLine = rLine;
            this.fromFront = fromFront;
            this.speedModifier = speedModifier;
        }

        public void Update(float timeDifference)
        {
            //speedModifier += timeDifference * speedModifier;
            float neededToShorten = DyingRainbowLine.shortenBy * (timeDifference * speedModifier);

            if (fromFront)
            {
                for (int i = 0; i < rainbowLine.Count; ++i)
                {
                    float length = rainbowLine[i].halfLineVector.Length();
                    if (length <= minLength)
                    {
                        neededToShorten -= length;
                        rainbowLine.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        rainbowLine[i].ShortenFromBack(neededToShorten / length);
                        i = rainbowLine.Count;
                        break;
                    }
                }
            }
            else
            {
                for (int i = rainbowLine.Count - 1; i >= 0; --i)
                {
                    float length = rainbowLine[i].halfLineVector.Length();
                    if (length <= minLength)
                    {
                        neededToShorten -= length;
                        rainbowLine.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        rainbowLine[i].ShortenFromFront(neededToShorten / length);
                        i = rainbowLine.Count;
                        break;
                    }
                }
            }



        }

    }
}
