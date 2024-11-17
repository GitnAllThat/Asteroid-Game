using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Shapes;
using Things;
using Global_Data;
using _2DLevelCreator;
using LoadSpace;

namespace Asteroid_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class AsteroidGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;



        public Random randNo = new Random();

        public List<List<RainbowLine>> _touchScreenLine = new List<List<RainbowLine>>();
        public List<Line> _gameBoundaries = new List<Line>();
        public List<Line> _dottedLines = new List<Line>();
        public List<DyingRainbowLine> listDyingRainbowLines = new List<DyingRainbowLine>();
        public List<Circle> listStars = new List<Circle>();
        public List<PlusOne> listPlusOne = new List<PlusOne>();
        RainbowColours rainbowColours = new RainbowColours();
        public List<Projectile> listProjectiles = new List<Projectile>();
        public List<BlackHole> listBlackHoles = new List<BlackHole>();

        public Thing2D thing2D_Meteor;
        public Thing2D thing2D_BlackHole;
        public Thing2D thing2D_Earth;
        public Thing2D thing2D_Star;
        public Thing2D thing2D_PlusOne;
        public Thing2D thing2D_Backdrop;
        public Thing2D thing2D_EarthExplode01;

        public Earth earth;
        Backdrop backdrop;





        public int score = 0;
        public static float xBound = 12;
        public static float yBound = 12;
        public float lesserBound;
        public float halfBoundsX;
        public float halfBoundsY;

        //Game Attributes
        bool gameOver = false;
        float autoAim = 0.1f;
        float gravitationalPull = 1;
        float earthGravitationalPull = 2;
        public float plusOneSize = 1;
        public float blackHoleSize = 1.8f;
        public float blackHoleSpeedMin = 0.3f;
        public float blackHoleSpeedMax = 0.9f;
        float createBlackHoleDelay = 1;
        int createBlackHoleDelayTimeMin = 2;
        int createBlackHoleDelayTimeMax = 4;
        int maxNoBlackHoles = 1;
        public float TimeTillNextProjectile = 1;
        public float TimeTillNextProjectileMin = 3;
        public float TimeTillNextProjectileMax = 4;
        public float projectileSpeedMin = 1.1f;
        public float projectileSpeedMax = 1.5f;
        public float projectileSize = 0.5f;
        float touchStrokeLength = 0;
        float touchStrokeLengthMax = 10;
        float minLineLength = 0.0075f;
        int lineThickness = 2;
        float delayBeforeGameOverScreenMax = 4;
        float delayBeforeGameOverScreen = 4;
        float lineUsed = 0;
        float lineLengthBeforeAsteroid = 2;

        public float StartAutoAim = 0.1f;
        public float StartTimeTillNextProjectileMin = 3;
        public float StartTimeTillNextProjectileMax = 4;
        public float StartprojectileSpeedMin;
        public float StartprojectileSpeedMax;
        public float StarttouchStrokeLengthMax;
        public float StartlineLengthBeforeAsteroid;
        public float gameTimeElapsed = 0;
        public float MaximumSpeed;
        public float MinLineLengthBeforeAsteroid;

        int lastGameScore = 0;

        public SpriteFont font;
        public Camera camera;
        public GameInput gameInput;
        public BasicEffect basicEffect;

        public float timeDifference;    //Reimplement Check if working
        //public Effect effect;

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Content = new ContentManager(this.Services, "Content");
            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            font = Content.Load<SpriteFont>("ArialFont");
            //Globals.effect = Globals.content.Load<Effect>("MyHLSL");
            camera = new Camera(1.777f);
            camera.CameraPosition = new Vector3(0, 0, 40);
            gameInput = new GameInput();
            //GraphicsDevice = this.graphics.GraphicsDevice;

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Projection = camera.ProjectionMatrix;
            basicEffect.TextureEnabled = true;

            //effect = Content.Load<BasicEffect>("MyHLSL_Old"); //reimplement was Effect instead of BasicEffect

            Load.Load_Game_Materials(Content);



            //Game attributes are modified by screen size

            xBound = GraphicsDevice.Viewport.Width / 70;
            yBound = GraphicsDevice.Viewport.Height / 70;

            lesserBound = xBound;
            if (yBound < xBound) lesserBound = yBound;

            StartprojectileSpeedMin = lesserBound / 24;        //i.e. 0.5f
            StartprojectileSpeedMax = lesserBound / 12;        //i.e. 1
            StarttouchStrokeLengthMax = lesserBound / 1.5f;    //i.e. 6
            StartlineLengthBeforeAsteroid = lesserBound / 6;   //i.e. 2
            MaximumSpeed = lesserBound / 4.5f;
            MinLineLengthBeforeAsteroid = lesserBound / 12;
            plusOneSize = lesserBound * 0.125f;
            blackHoleSize = lesserBound * 0.2f;
            blackHoleSpeedMin = lesserBound * 0.035f;
            blackHoleSpeedMax = lesserBound * 0.11f;
            projectileSpeedMin = lesserBound * 0.138f;
            projectileSpeedMax = lesserBound * 0.188f;
            projectileSize = lesserBound * 0.0626f;
            touchStrokeLengthMax = lesserBound * 1.5f;
            minLineLength = lesserBound * minLineLength;

            //gravitationalPull = 1;
            //earthGravitationalPull = 2;



            thing2D_Meteor = new Thing2D("Meteor_Thing2D", Material.GetUniqueIdentifierByName("mat_Meteor"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_BlackHole = new Thing2D("BlackHole_Thing2D", Material.GetUniqueIdentifierByName("mat_Black_Hole"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_Earth = new Thing2D("Earth_Thing2D", Material.GetUniqueIdentifierByName("mat_Earth"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_Star = new Thing2D("Star_Thing2D", Material.GetUniqueIdentifierByName("mat_Star"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_PlusOne = new Thing2D("PlusOne_Thing2D", Material.GetUniqueIdentifierByName("mat_Plus_One"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_Backdrop = new Thing2D("Backdrop_Thing2D", Material.GetUniqueIdentifierByName("mat_Backdrop"), new VertexPositionTextureArray(), new List<Thing2D>());
            thing2D_EarthExplode01 = new Thing2D("EarthExplode01_Thing2D", Material.GetUniqueIdentifierByName("mat_EarthExplode01"), new VertexPositionTextureArray(), new List<Thing2D>());

            backdrop = new Backdrop(new Vector3(0, 0, 0), new Vector3(xBound, yBound, 1), thing2D_Backdrop);
            backdrop.SetTransparency(0.6f);

            earth = new Earth(Vector3.Zero, 0, new Vector3(lesserBound * 0.1875f, lesserBound * 0.1875f, 1), thing2D_Earth);

 



            halfBoundsX = xBound * 0.5f;
            halfBoundsY = yBound * 0.5f;
            Color boarderColour = new Color(111, 111, 111, 111);
            Line topL_topR = new Line(new Vector3(-halfBoundsX, halfBoundsY, 0), new Vector3(halfBoundsX, halfBoundsY, 0), boarderColour, boarderColour, 2);
            Line botL_botR = new Line(new Vector3(-halfBoundsX, -halfBoundsY, 0), new Vector3(halfBoundsX, -halfBoundsY, 0), boarderColour, boarderColour, 2);
            Line lefT_lefB = new Line(new Vector3(-halfBoundsX, halfBoundsY, 0), new Vector3(-halfBoundsX, -halfBoundsY, 0), boarderColour, boarderColour, 2);
            Line rigT_rigB = new Line(new Vector3(halfBoundsX, halfBoundsY, 0), new Vector3(halfBoundsX, -halfBoundsY, 0), boarderColour, boarderColour, 2);

            _gameBoundaries.Add(topL_topR);
            _gameBoundaries.Add(botL_botR);
            _gameBoundaries.Add(lefT_lefB);
            _gameBoundaries.Add(rigT_rigB);


            float dottedBoundsX = (xBound * 0.5f) - (blackHoleSize);
            float dottedBoundsY = (yBound * 0.5f) - (blackHoleSize);
            _dottedLines.Add(new Line(new Vector3(-dottedBoundsX, dottedBoundsY, 0), new Vector3(dottedBoundsX, dottedBoundsY, 0), new Color(111, 111, 111, 155), 1));
            _dottedLines.Add(new Line(new Vector3(-dottedBoundsX, -dottedBoundsY, 0), new Vector3(dottedBoundsX, -dottedBoundsY, 0), new Color(111, 111, 111, 155), 1));
            _dottedLines.Add(new Line(new Vector3(-dottedBoundsX, dottedBoundsY, 0), new Vector3(-dottedBoundsX, -dottedBoundsY, 0), new Color(111, 111, 111, 155), 1));
            _dottedLines.Add(new Line(new Vector3(dottedBoundsX, dottedBoundsY, 0), new Vector3(dottedBoundsX, -dottedBoundsY, 0), new Color(111, 111, 111, 155), 1));





            rainbowColours.index = 1;
            _touchScreenLine.Add(new List<RainbowLine>());

            #region Setup Stars
            //Setup Stars
            float mult = 10000;
            float invMult = 1 / mult;
            float posXMax = halfBoundsX * mult;
            float posYMax = halfBoundsY * mult;
            for (int i = 0; i < 1000; ++i)
            {
                float x = randNo.Next(0, (int)posXMax) * invMult;
                if (randNo.Next(0, 2) == 1) x *= -1;
                float y = randNo.Next(0, (int)posYMax) * invMult;
                if (randNo.Next(0, 2) == 1) y *= -1;

                float radius = randNo.Next(2, 5) * 0.01f;
                int alpha = randNo.Next(20, 70);
                int rg = randNo.Next(166, 255);
                listStars.Add(new Circle(new Vector3(x, y, 0), radius, Circle.DrawStyle.Fill, new Color(rg, rg, 255, alpha), new Color(rg, rg, 255, alpha)));
            }
            #endregion

            ResetGame();

            base.Initialize();
        }

        public void ResetGame()
        {
            score = 0;
            gameTimeElapsed = 0;

            _touchScreenLine = new List<List<RainbowLine>>();
            listDyingRainbowLines = new List<DyingRainbowLine>();
            listPlusOne = new List<PlusOne>();
            listProjectiles = new List<Projectile>();
            listBlackHoles = new List<BlackHole>();

            delayBeforeGameOverScreen = delayBeforeGameOverScreenMax;
            gameOver = false;
            lineUsed = 0;



            autoAim = StartAutoAim;
            TimeTillNextProjectileMin = StartTimeTillNextProjectileMin;
            TimeTillNextProjectileMax = StartTimeTillNextProjectileMax;
            projectileSpeedMin = StartprojectileSpeedMin;
            projectileSpeedMax = StartprojectileSpeedMax;
            touchStrokeLengthMax = StarttouchStrokeLengthMax;
            lineLengthBeforeAsteroid = StartlineLengthBeforeAsteroid;

            lastGameScore = 0;
        }













        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }


        protected override void UnloadContent()
        {
        }
    }
}
