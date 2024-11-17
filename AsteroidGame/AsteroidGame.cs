using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroid_Game
{
    public partial class AsteroidGame : Game
    {
        public AsteroidGame()
        {

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";




            graphics.PreferMultiSampling = true;
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            graphics.ApplyChanges();
            graphics.GraphicsDevice.Reset(graphics.GraphicsDevice.PresentationParameters);
            //graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;    // Allows alpha
            graphics.ApplyChanges();


            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
            return;
        }



    }
}
