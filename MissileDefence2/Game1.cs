using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RC_Framework;

namespace MissileDefence2.MacOS
{
    /// enum to manage game state
    public enum GameState
    {
        SplashScreen,
        Level1,
        Level2,
        Pause,
        Lose,
        Win,
        Credit
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Some constants
        public static float MAX_ROTATION_DEGREE = (float)Math.PI / 4;
        public static float MIN_ROTATION_DEGREE = - (float)Math.PI / 4;
        public static float DELTA_ROTATION = (float)Math.PI / 90;
        public static float MISSILE_DISPLAY_ANGLE = -(float)Math.PI / 2;
        public static float THREAT_DISPLAY_ANGLE = (float)Math.PI / 2;
        public static int LEVEL1_SPEED = 4;
        public static int LEVEL2_SPEED = 8;
        public static int LEVEL3_SPEED = 10;

        //GameState
        GameState gameState;

        //Textures
        Texture2D textureSplashScreen;
        Texture2D textureLevel1BackGround;
        Texture2D textureMissile;
        Texture2D textureThreat1;
        Texture2D textureThreat2;
        Texture2D textureThreat3;

        //Backgrounds
        ImageBackground splashScreenBackGround;
        ImageBackground level1BackGround;

        //Sprites
        Sprite3 spriteMissile;
        Sprite3 spriteThreat1;
        Sprite3 spriteThreat2;
        Sprite3 spriteThreat3;

        //SpriteLists
        SpriteList threatList;

        //font
        SpriteFont font;

        //Keyboard input
        KeyboardState currentKeyState;
        KeyboardState prevKeyState;

        //bools
        bool missileLaunched;
        bool showBoundingBox;

        //Random number generator
        Random random;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Set Game State to Menu to begin with
            gameState = GameState.SplashScreen;
            showBoundingBox = false;
            missileLaunched = false;
            threatList = new SpriteList(6);
            random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 
            LineBatch.init(GraphicsDevice);

            //Load all Textures
            textureSplashScreen = Content.Load<Texture2D>("Images/SplashScreen");
            textureLevel1BackGround = Content.Load<Texture2D>("Images/BackgroundL1");
            textureMissile = Content.Load<Texture2D>("Images/Missile");
            textureThreat1 = Content.Load<Texture2D>("Images/Threat1");
            textureThreat2 = Content.Load<Texture2D>("Images/Threat2");
            textureThreat3 = Content.Load<Texture2D>("Images/Threat3");

            //Create backgrounds from texture
            splashScreenBackGround = new ImageBackground(textureSplashScreen, Color.White, GraphicsDevice);
            level1BackGround = new ImageBackground(textureLevel1BackGround, Color.White, GraphicsDevice);

            //Create sprites from texture
            spriteMissile = new Sprite3(true, textureMissile, 380, 400);
            //spriteMissile.setBBandHSFractionOfTexCentered(1.0f);
            spriteMissile.setHSoffset(new Vector2(textureMissile.Width / 2, textureMissile.Height / 2));
            spriteMissile.setDisplayAngleRadians(MISSILE_DISPLAY_ANGLE);
            spriteMissile.setMoveAngleRadians(spriteMissile.getDisplayAngleRadians());
            spriteMissile.setMoveSpeed(LEVEL1_SPEED);

            spriteThreat1 = new Sprite3(false, textureThreat1, 0, 0);
            spriteThreat1.setHSoffset(new Vector2(textureThreat1.Width / 2, textureThreat1.Height / 2));
            spriteThreat1.setMoveSpeed(LEVEL1_SPEED);
            spriteThreat1.launched = false;
            spriteThreat2 = new Sprite3(false, textureThreat2, 0, 0);
            spriteThreat2.setHSoffset(new Vector2(textureThreat2.Width / 2, textureThreat2.Height / 2));
            spriteThreat2.setMoveSpeed(LEVEL1_SPEED);
            spriteThreat2.launched = false;
            spriteThreat3 = new Sprite3(false, textureThreat3, 0, 0);
            spriteThreat3.setHSoffset(new Vector2(textureThreat3.Width / 2, textureThreat3.Height / 2));
            spriteThreat3.setMoveSpeed(LEVEL1_SPEED);
            spriteThreat3.launched = false;

            //Add threat sprites to spritelist
            threatList.addSprite(spriteThreat1);
            threatList.addSprite(spriteThreat2);
            threatList.addSprite(spriteThreat3);
            

            font = Content.Load<SpriteFont>("Fonts/Font");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            prevKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Check for gameState value and call appropriate update functions
            switch (gameState)
            {
                case GameState.SplashScreen:
                    UpdateSplashScreen();
                    break;
                
                case GameState.Level1:
                    UpdateLevel1();
                    break;

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //TODO: Add your drawing code here
            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.SplashScreen:
                    DrawSplashScreen();
                    break;

                case GameState.Level1:
                    DrawLevel1();
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateSplashScreen()
        {
            if (currentKeyState.IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Level1;
            }
        }

        private void UpdateLevel1()
        {
            //Check for b key
            if (prevKeyState.IsKeyUp(Keys.B) && currentKeyState.IsKeyDown(Keys.B)) 
            {
                showBoundingBox = !showBoundingBox;
            }
            //Update missile
            if (currentKeyState.IsKeyDown(Keys.Right) && spriteMissile.getDisplayAngleRadians() < MAX_ROTATION_DEGREE + MISSILE_DISPLAY_ANGLE)
            {
                spriteMissile.setDisplayAngleRadians(spriteMissile.getDisplayAngleRadians() + DELTA_ROTATION);
                spriteMissile.setMoveAngleRadians(spriteMissile.getDisplayAngleRadians());
                //spriteMissile.getBoundingBoxAA();
            }
            if (currentKeyState.IsKeyDown(Keys.Left) && spriteMissile.getDisplayAngleRadians() > MIN_ROTATION_DEGREE + MISSILE_DISPLAY_ANGLE)
            {
                spriteMissile.setDisplayAngleRadians(spriteMissile.getDisplayAngleRadians() - DELTA_ROTATION);
                spriteMissile.setMoveAngleRadians(spriteMissile.getDisplayAngleRadians());
                //spriteMissile.getBoundingBoxAA();
            }

            if (currentKeyState.IsKeyDown(Keys.Up))
            {
                missileLaunched = true;
            }
            if (missileLaunched)
            {
                spriteMissile.moveByAngleSpeed();
            }
            Rectangle missileAABB = spriteMissile.getBoundingBoxAA();
            if (!missileAABB.Intersects(GraphicsDevice.Viewport.Bounds))
            {
                ResetMissile();
            }

            //update threats
            if (!ThreatLaunched())
            {
                random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                for (int i = 0; i < threatList.count(); i++)
                {
                    if (threatList[i].visible == false) 
                    {
                        threatList[i].setPos(new Vector2(random.Next(200, 800 - 200), 0));
                        threatList[i].setDisplayAngleRadians((float)random.NextDouble() * (float)(3 * Math.PI / 4 - Math.PI / 4) + (float)Math.PI / 4);
                        threatList[i].setMoveAngleRadians(threatList[i].getDisplayAngleRadians());
                        threatList[i].visible = true;
                        threatList[i].launched = true;
                    }
                }
            }
            else 
            {
                for (int i = 0; i < threatList.count(); i++) 
                {
                    if (threatList[i].visible == true)
                    {
                        threatList[i].moveByAngleSpeed();
                    }
                    Rectangle threatAABB = threatList[i].getBoundingBoxAA();
                    if (!threatAABB.Intersects(GraphicsDevice.Viewport.Bounds)) //threat out of view
                    {
                        threatList[i].launched = false;
                        threatList[i].visible = false;

                    }

                }
            }
            //Check collision with missile
            int collision = threatList.collisionWithRect(missileAABB);
            if (collision != -1 && missileLaunched) //collosion between missile and threat
            {
                ResetMissile();
                threatList[collision].visible = false;
                threatList[collision].launched = false;
            }

        }

        private void DrawSplashScreen()
        {
            splashScreenBackGround.Draw(spriteBatch);
        }

        private void DrawLevel1()
        {
            level1BackGround.Draw(spriteBatch);
            spriteMissile.draw(spriteBatch);
            spriteBatch.DrawString(font, "active: " + threatList.count(), new Vector2(50, 50), Color.White);
            threatList.drawAll(spriteBatch);

            if (showBoundingBox)
            {
                spriteMissile.drawInfo(spriteBatch, Color.White, Color.Black);
                spriteMissile.drawRect4(spriteBatch, Color.Red);
                threatList.drawInfo(spriteBatch, Color.White, Color.Red);
                threatList.drawRect4(spriteBatch, Color.Blue);
            }

        }

        private bool ThreatLaunched()
        {
            for (int i = 0; i < threatList.count(); i++)
            {
                if (threatList[i].launched)
                    return true;
            }
            return false;
        }

        private void ResetMissile() 
        {
            spriteMissile.setPos(380, 400);
            spriteMissile.setDisplayAngleRadians(MISSILE_DISPLAY_ANGLE);
            spriteMissile.setMoveAngleRadians(spriteMissile.getDisplayAngleRadians());
            missileLaunched = false;
        }
    }
}
