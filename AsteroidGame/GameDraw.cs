using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _2d_Objects;

namespace Asteroid_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class AsteroidGame : Microsoft.Xna.Framework.Game
    {
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(0, 0, 0));
            this.GraphicsDevice.BlendState = BlendState.NonPremultiplied;    // Allows alpha
            


            backdrop.Draw(this);
            //for (int i = 0, iMax = listStars.Count; i < iMax; ++i) { listStars[i].Draw(this.XnaWindow); }



            earth.Draw(this);
            if (gameOver)
            {
                thing2D_EarthExplode01.Draw(this, earth.transform, Matrix.CreateRotationZ(earth.impactRotation));
            }

            for (int i = 0; i < _dottedLines.Count; i++) { _dottedLines[i].Draw(this); }
            for (int i = 0; i < _touchScreenLine.Count; i++) for (int j = 0; j < _touchScreenLine[i].Count; j++) { _touchScreenLine[i][j].Draw(this); }
            for (int i = 0; i < _gameBoundaries.Count; i++) { _gameBoundaries[i].Draw(this); }
            for (int i = 0, iCount = listBlackHoles.Count; i < iCount; ++i) listBlackHoles[i].Draw(this);
            for (int i = 0, iMax = listProjectiles.Count; i < iMax; ++i) { listProjectiles[i].Draw(this); }
            for (int i = 0, iMax = listPlusOne.Count; i < iMax; ++i) { listPlusOne[i].Draw(this); }



            



            /*
            Globals.spriteBatch.Begin();

            float height = 10;
            Globals.spriteBatch.DrawString(Globals.font, "Score: " + score, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "Game Time Elapsed: " + gameTimeElapsed, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "Line Used: " + lineUsed, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "Auto Aim: " + autoAim, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "TimeTillNextProjectileMin: " + TimeTillNextProjectileMin, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "TimeTillNextProjectileMax: " + TimeTillNextProjectileMax, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "projectileSpeedMin: " + projectileSpeedMin, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "projectileSpeedMax: " + projectileSpeedMax, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "lineLengthBeforeAsteroid: " + lineLengthBeforeAsteroid, new Vector2(20, height += 20), Color.White);
            Globals.spriteBatch.DrawString(Globals.font, "No. Line Lists: " + _touchScreenLine.Count, new Vector2(20, height += 20), Color.White);


            //Globals.spriteBatch.DrawString(Globals.font, "Mouse Down: " + Globals.gameInput.MouseLeftIsDown, new Vector2(20, height += 20), Color.White);
            //Globals.spriteBatch.DrawString(Globals.font, "Mouse Pressed: " + Globals.gameInput.MouseLeftPressed, new Vector2(20, height += 20), Color.White);
            //Globals.spriteBatch.DrawString(Globals.font, "Mouse Released: " + Globals.gameInput.MouseLeftReleased, new Vector2(20, height += 20), Color.White);


            //for(int i =0; i<listString.Count;++i)this.XnaWindow.spriteBatch.DrawString(this.XnaWindow.font, listString[i], new Vector2(20, height += 20), Color.White);

            Globals.spriteBatch.End();
            
            */



            Collision_Methods.DrawDebuggingTools(this);   //Draws any debugging tools


            base.Draw(gameTime);
        }
    }
}
