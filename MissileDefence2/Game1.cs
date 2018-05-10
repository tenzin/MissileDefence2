using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using RC_Framework;

namespace MissileDefence2.MacOS
{
    /// enum to manage game state
    public enum GameState
    {
        SplashScreen,
        Level1,
        Level1Win,
        Level1Lose,
        Level2,
        Level2Win,
        Level2Lose,
        Level3,
        Level3Win,
        Level3Lose,
        Pause,
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
        //Rotation info for threats and missile
        public static float MAX_ROTATION_DEGREE = (float)Math.PI / 4;
        public static float MIN_ROTATION_DEGREE = - (float)Math.PI / 4;
        public static float DELTA_ROTATION = (float)Math.PI / 90;
        public static float MISSILE_DISPLAY_ANGLE = -(float)Math.PI / 2;
        public static float THREAT_DISPLAY_ANGLE = (float)Math.PI / 2;

        //Number of waves for each level
        public static int LEVEL_1_WAVE = 5;
        public static int LEVEL_2_WAVE = 7;
        public static int LEVEL_3_WAVE = 9;

        //Threat speed for each level
        //public static int LEVEL1_SPEED = 2;
        public static int LEVEL2_SPEED = 5;
        public static int LEVEL3_SPEED = 8;

        //Missile Speed
        public static int MISSILE_SPEED = 8;

        //Level 1 Threat count
        public static int LEVEL_1_THREAT_COUNT = 5;
        public static int LEVEL_2_THREAT_COUNT = 3;
        public static int LEVEL_3_THREAT_COUNT = 5;

        //GameState
        GameState gameState;

        //Textures
        //Backgrounds
        Texture2D textureSplashScreen;
        Texture2D textureLevel1BackGround;
        Texture2D textureMissile;

        // threats
        Texture2D textureThreat1;
        Texture2D textureThreat2;
        Texture2D textureThreat3;
        Texture2D textureThreatBallon;

        //cities
        Texture2D textureCity1;
        Texture2D textureCity2;
        Texture2D textureCity3;
        Texture2D textureCity4;

        //Backgrounds
        ImageBackground splashScreenBackGround;
        ImageBackground level1BackGround;

        //Sprites
        Sprite3 spriteMissile;

        //threat sprites
        Sprite3 spriteThreat1;
        Sprite3 spriteThreat2;
        Sprite3 spriteThreat3;
        Sprite3 spriteBallon;

        //Cities
        Sprite3 spriteCity1;
        Sprite3 spriteCity2;
        Sprite3 spriteCity3;
        Sprite3 spriteCity4;

        //SpriteLists
        SpriteList level1ThreatList;
        SpriteList level2ThreatList;
        SpriteList level3ThreatList;

        //font
        SpriteFont font;

        //Sounds
        SoundEffect explosion;

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
            level1ThreatList = new SpriteList(LEVEL_1_THREAT_COUNT);
            level2ThreatList = new SpriteList(LEVEL_2_THREAT_COUNT);
            level3ThreatList = new SpriteList(LEVEL_3_THREAT_COUNT);

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
            textureThreatBallon = Content.Load<Texture2D>("Images/Ballon");
            textureCity1 = Content.Load<Texture2D>("Images/City1");
            textureCity2 = Content.Load<Texture2D>("Images/City2");
            textureCity3 = Content.Load<Texture2D>("Images/City3");
            textureCity4 = Content.Load<Texture2D>("Images/City4");

            //Create backgrounds from texture
            splashScreenBackGround = new ImageBackground(textureSplashScreen, Color.White, GraphicsDevice);
            level1BackGround = new ImageBackground(textureLevel1BackGround, Color.White, GraphicsDevice);

            //Create sprites from texture
            spriteMissile = new Sprite3(true, textureMissile, 380, 400);
            spriteMissile.setHSoffset(new Vector2(textureMissile.Width / 2, textureMissile.Height / 2));
            spriteMissile.setDisplayAngleRadians(MISSILE_DISPLAY_ANGLE);
            spriteMissile.setMoveAngleRadians(spriteMissile.getDisplayAngleRadians());
            spriteMissile.setMoveSpeed(MISSILE_SPEED);

            //City 
            spriteCity1 = new Sprite3(true, textureCity1, 30, 355);
            spriteCity1.setBBToTexture();
            spriteCity2 = new Sprite3(true, textureCity2, 195, 355);
            spriteCity2.setBBToTexture();
            spriteCity3 = new Sprite3(true, textureCity3, 475, 355);
            spriteCity3.setBBToTexture();
            spriteCity4 = new Sprite3(true, textureCity4, 651, 355);
            spriteCity4.setBBToTexture();

            //create threat sprites and load threatlists for all levels -- not very good but simpler to load all content in the begining
            //Level1 threatlist
            for (int i = 0; i < LEVEL_1_THREAT_COUNT; i++)
            {
                Sprite3 temp = new Sprite3(false, textureThreatBallon, 0, 0);
                temp.launched = false;
                temp.setHSoffset(new Vector2(0, textureThreatBallon.Height));
                //temp.setDeltaSpeed(new Vector2(0, 1));
                level1ThreatList.addSpriteReuse(temp);
            }

            //Level2 threatlist
            spriteThreat1 = new Sprite3(false, textureThreat1, 0, 0);
            spriteThreat1.setHSoffset(new Vector2(textureThreat1.Width / 2, textureThreat1.Height / 2));
            spriteThreat1.setMoveSpeed(LEVEL2_SPEED);
            spriteThreat1.launched = false;
            spriteThreat2 = new Sprite3(false, textureThreat2, 0, 0);
            spriteThreat2.setHSoffset(new Vector2(textureThreat2.Width / 2, textureThreat2.Height / 2));
            spriteThreat2.setMoveSpeed(LEVEL2_SPEED);
            spriteThreat2.launched = false;
            spriteThreat3 = new Sprite3(false, textureThreat3, 0, 0);
            spriteThreat3.setHSoffset(new Vector2(textureThreat3.Width / 2, textureThreat3.Height / 2));
            spriteThreat3.setMoveSpeed(LEVEL2_SPEED);
            spriteThreat3.launched = false;
            level2ThreatList.addSprite(spriteThreat1);
            level2ThreatList.addSprite(spriteThreat2);
            level2ThreatList.addSprite(spriteThreat3);

            
            //Create font
            font = Content.Load<SpriteFont>("Fonts/Font");

            //Creat sound
            explosion = Content.Load<SoundEffect>("Sounds/Explosion");
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

            //Check for b key
            if (prevKeyState.IsKeyUp(Keys.B) && currentKeyState.IsKeyDown(Keys.B))
            {
                showBoundingBox = !showBoundingBox;
            }
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

                case GameState.Level1Win:
                    break;

                case GameState.Level2:
                    UpdateLevel2();
                    break;

                case GameState.Level3:
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
                    level1BackGround.Draw(spriteBatch);
                    spriteCity1.Draw(spriteBatch);
                    spriteCity2.Draw(spriteBatch);
                    spriteCity3.Draw(spriteBatch);
                    spriteCity4.Draw(spriteBatch);
                    spriteMissile.Draw(spriteBatch);
                    DrawLevel1();
                    break;
                case GameState.Level2:
                    level1BackGround.Draw(spriteBatch);
                    spriteCity1.Draw(spriteBatch);
                    spriteCity2.Draw(spriteBatch);
                    spriteCity3.Draw(spriteBatch);
                    spriteCity4.Draw(spriteBatch);
                    spriteMissile.Draw(spriteBatch);
                    DrawLevel2();
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Splash Screen
        private void UpdateSplashScreen()
        {
            if (currentKeyState.IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Level1;
            }
        }

        private void DrawSplashScreen()
        {
            splashScreenBackGround.Draw(spriteBatch);
        }
        #endregion

        #region Level1
        private void UpdateLevel1()
        {
            //Update missile
            UpdateMissile();

            //update threats
            if (!ThreatLaunched(level1ThreatList)) //all threats are not active
            {
                ResetLevel1Threats(); //reset and make them active
            }
            else //Move threat sprites and if they get out of bounds, make them inactive
            {
                for (int i = 0; i < LEVEL_1_THREAT_COUNT; i++)
                {
                    if (level1ThreatList[i].visible == true)
                    {
                        level1ThreatList[i].moveByDeltaXY();
                    }
                    Rectangle threatAABB = level1ThreatList[i].getBoundingBoxAA();
                    if (!threatAABB.Intersects(GraphicsDevice.Viewport.Bounds)) //threat out of view
                    {
                        level1ThreatList[i].launched = false;
                        level1ThreatList[i].visible = false;

                    }
                }
            }

            //collision detection
            //Check collision with missile
            Rectangle missileAABB = spriteMissile.getBoundingBoxAA();
            int collision = level1ThreatList.collisionWithRect(missileAABB);
            if (collision != -1 && missileLaunched) //collosion between missile and threat
            {
                ResetMissile();
                level1ThreatList[collision].visible = false;
                level1ThreatList[collision].launched = false;
                explosion.Play();
            }

            //check collision with cities
            List<Rectangle> cityBounds = new List<Rectangle>(4);
            cityBounds.Add(spriteCity1.bounds);
            cityBounds.Add(spriteCity2.bounds);
            cityBounds.Add(spriteCity3.bounds);
            cityBounds.Add(spriteCity4.bounds);

            foreach (Rectangle rectangle in cityBounds)
            {
                collision = level1ThreatList.collisionWithRect(rectangle);
                if (collision != -1) //collosion between city and threat
                {
                    level1ThreatList[collision].visible = false;
                    level1ThreatList[collision].launched = false;
                    explosion.Play();
                }
            }
        }
        private void DrawLevel1()
        {
            level1ThreatList.drawAll(spriteBatch);
            spriteBatch.DrawString(font, "active: " + level1ThreatList.count(), new Vector2(50, 50), Color.White);
        }

        private void ResetLevel1Threats()
        {
            random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            for (int i = 0; i < LEVEL_1_THREAT_COUNT; i++)
            {
                if (level1ThreatList[i].visible == false)
                {
                    level1ThreatList[i].setPos(new Vector2(random.Next(20, 800 - 20), 0));
                    level1ThreatList[i].setDeltaSpeed(new Vector2(0, 0.5f + (float)random.NextDouble()));
                    level1ThreatList[i].visible = true;
                    level1ThreatList[i].launched = true;
                }
            }
        }

        #endregion

        #region Level1Win
        private void UpdateLevel1Win()
        {
            if (currentKeyState.IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Level2;
                ResetLevel2Threats();
            }
        }
        #endregion


        #region Level2
        private void UpdateLevel2()
        {
            //Update missile
            UpdateMissile();

            //update threats
            if (!ThreatLaunched(level2ThreatList)) //all threats are not active
            {
                ResetLevel2Threats(); //reset and make them active
            }
            else 
            {
                for (int i = 0; i < LEVEL_2_THREAT_COUNT; i++) 
                {
                    if (level2ThreatList[i].visible == true)
                    {
                        level2ThreatList[i].moveByAngleSpeed();
                    }
                    Rectangle threatAABB = level2ThreatList[i].getBoundingBoxAA();
                    if (!threatAABB.Intersects(GraphicsDevice.Viewport.Bounds)) //threat out of view
                    {
                        level2ThreatList[i].launched = false;
                        level2ThreatList[i].visible = false;

                    }
                }
            }
            //collision detection
            //Check collision with missile
            Rectangle missileAABB = spriteMissile.getBoundingBoxAA();
            int collision = level2ThreatList.collisionWithRect(missileAABB);
            if (collision != -1 && missileLaunched) //collosion between missile and threat
            {
                ResetMissile();
                level2ThreatList[collision].visible = false;
                level2ThreatList[collision].launched = false;
                explosion.Play();
            }

            //check collision with cities
            List<Rectangle> cityBounds = new List<Rectangle>(4);
            cityBounds.Add(spriteCity1.bounds);
            cityBounds.Add(spriteCity2.bounds);
            cityBounds.Add(spriteCity3.bounds);
            cityBounds.Add(spriteCity4.bounds);

            foreach (Rectangle rectangle in cityBounds)
            {
                collision = level2ThreatList.collisionWithRect(rectangle);
                if (collision != -1) //collosion between city and threat
                {
                    level2ThreatList[collision].visible = false;
                    level2ThreatList[collision].launched = false;
                    explosion.Play();
                }
            }
        }

        private void DrawLevel2()
        {
            
            spriteBatch.DrawString(font, "active: " + level2ThreatList.count(), new Vector2(50, 50), Color.White);
            level2ThreatList.drawAll(spriteBatch);
            if (showBoundingBox)
            {
                spriteMissile.drawInfo(spriteBatch, Color.White, Color.Black);
                spriteMissile.drawRect4(spriteBatch, Color.Red);
                level2ThreatList.drawInfo(spriteBatch, Color.White, Color.Red);
                level2ThreatList.drawRect4(spriteBatch, Color.Blue);
                spriteCity1.drawInfo(spriteBatch, Color.White, Color.Red);
                spriteCity2.drawInfo(spriteBatch, Color.White, Color.Red);
                spriteCity3.drawInfo(spriteBatch, Color.White, Color.Red);
                spriteCity4.drawInfo(spriteBatch, Color.White, Color.Red);
            }

        }

        public void ResetLevel2Threats()
        {
            random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            for (int i = 0; i < LEVEL_2_THREAT_COUNT; i++)
            {
                if (level2ThreatList[i].visible == false)
                {
                    level2ThreatList[i].setPos(new Vector2(random.Next(200, 800 - 200), 0));
                    level2ThreatList[i].setDisplayAngleRadians((float)random.NextDouble() * (float)(3 * Math.PI / 4 - Math.PI / 4) + (float)Math.PI / 4);
                    level2ThreatList[i].setMoveAngleRadians(level2ThreatList[i].getDisplayAngleRadians());
                    level2ThreatList[i].visible = true;
                    level2ThreatList[i].launched = true;
                }
            }
        }
        #endregion

        private void UpdateMissile()
        {
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
        }

        private bool ThreatLaunched(SpriteList threatList)
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
