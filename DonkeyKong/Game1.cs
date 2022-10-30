using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace DonkeyKong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Song notEnraged;
        SoundEffect enemyDead;
        SoundEffect fireBall;
                               
        List<string> stringList = new List<string>();   
        string text;

        static Tiles[,] tileArray;

        Princess princess;
        Player player;
        Enemy enemy;
        List<Enemy> enemyList;

        Texture2D bridgeTexture;
        Texture2D emptyTexture;
        Texture2D ladderTexture;        
        Texture2D wallTile;
        Texture2D heartTexture;
        Texture2D gameOver;
        Texture2D win;
        Texture2D startText;
        int score = 0;
        
        Random random;

        Vector2 playerPosition;

        Animation bossAnimation;
        Animation enragedBossAnimation;

        int randomSpeed;

        float timer;
        float flameTimer;
        int enrageTimer;
        float invisTimer;       

        List<Flame> flameList;
        Flame flame;
        Texture2D flameAnimation;

        Vector2 flameVelocity;
        Vector2 flamePos;

        SpriteFont font;
       
        int lives = 5;
        public StreamReader StreamReader { get; private set; }

        enum GameState { start, inGame, gameOver, win};
        GameState currentState = GameState.start;        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {           
            timer = 60 * 4;
            flameTimer = 60 * 2;
            enrageTimer = 60 * 20;
            invisTimer = 60 * 5;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            notEnraged = Content.Load<Song>("034 Boss Introduction");
            enemyDead = Content.Load<SoundEffect>("bone-crack-121580");
            fireBall = Content.Load<SoundEffect>("FIreBall-Woosh");
            
            MediaPlayer.Play(notEnraged);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            bossAnimation = new Animation(Content, "Kong-Boss-SpriteSheet", 170f, 11, true);
            enragedBossAnimation = new Animation(Content, "Boss-SpriteSheet-Enraged", 170f, 11, true);
            heartTexture = Content.Load<Texture2D>("Heart");
            gameOver = Content.Load<Texture2D>("Game-Over-Boss");
            win = Content.Load<Texture2D>("Boss-Win");
            startText = Content.Load<Texture2D>("Start-Text");
            font = Content.Load<SpriteFont>("File");
                        
            flameAnimation = Content.Load<Texture2D>("NoobGodoter'sSpritesheet");
            flameList = new List<Flame>();

            bridgeTexture = Content.Load<Texture2D>("tile");
            emptyTexture = Content.Load<Texture2D>("empty");
            ladderTexture = Content.Load<Texture2D>("ladder");
            wallTile = Content.Load<Texture2D>("wall");
            random = new Random();

            StreamReader sr = new StreamReader("Map.txt");
            text = sr.ReadLine();
            while(!sr.EndOfStream)
            {
                stringList.Add(sr.ReadLine());
            }
            sr.Close();

            tileArray = new Tiles[stringList[0].Length, stringList.Count];
            enemyList = new List<Enemy>();
            
            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++) 
                {
                    Vector2 tilePosition = new Vector2(bridgeTexture.Width * i, bridgeTexture.Height * j);

                    if (stringList[j][i] == 'w')
                    {
                        tileArray[i, j] = new Tiles(bridgeTexture, tilePosition, true, false, false);
                    }
                    else if (stringList[j][i] == 'p')
                    {

                        tileArray[i, j] = new Tiles(emptyTexture, tilePosition, false, false,false);

                        playerPosition = new Vector2(bridgeTexture.Width * i, bridgeTexture.Height * j);

                        player = new Player(Content, playerPosition, "Player-Animation-Gun", 30f, 12, true);
                    }
                    else if (stringList[j][i] == '-')
                    {
                        tileArray[i, j] = new Tiles(emptyTexture, tilePosition, false, false, false);
                    }
                    else if (stringList[j][i] == 'x')
                    {                       
                        tileArray[i, j] = new Tiles(ladderTexture, tilePosition, false, false, false);
                    }
                    else if (stringList[j][i] == 'o')
                    {
                        tileArray[i, j] = new Tiles(wallTile, tilePosition, true, false, false);
                    }
                    else if (stringList[j][i] == 'e')
                    {
                        tileArray[i,j] = new Tiles(emptyTexture, tilePosition, false, false, false);
                                                
                    }
                    else if (stringList[j][i] == '@')
                    {
                        tileArray[i, j] = new Tiles(emptyTexture, tilePosition, false, true, false);
                    }
                    else if (stringList[j][i] == '#')
                    {
                        tileArray[i, j] = new Tiles(ladderTexture, tilePosition, false, true, true);
                    }
                    else if (stringList[j][i] == 's')
                    {
                        tileArray[i, j] = new Tiles(emptyTexture, tilePosition, false, false, false);
                        princess = new Princess(Content, "Princess-Spritesheet", new Vector2(bridgeTexture.Width * i, bridgeTexture.Height * j), 100f, 5, true);
                    }                    
                }                    
            }
            for (int e = 0; e < 1; e++)
            {
                randomSpeed = random.Next(60, 150);
                enemy = new Enemy(Content, new Vector2(bridgeTexture.Width, bridgeTexture.Height * 3), randomSpeed, "Enemy-Skeleton-Sheet", 90f, 4, true);

                enemyList.Add(enemy);                              
            }

            for (int f = 0; f < 1; f++)
            {
                //Random position of the flames
                int positionX = random.Next(0, 1280);                
                flamePos = new Vector2(positionX, 0);

                //Random speed of the flames                
                int velocityY = random.Next(2, 4);
                flameVelocity = new Vector2(0, velocityY);

                //The randomzied flame
                flame = new Flame(flamePos, flameVelocity, flameAnimation, 100f, 10, true);

                flameList.Add(flame);
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                        
            switch (currentState)
            {
                case GameState.start:
                   
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.inGame;                        
                    }
                    break;

                case GameState.inGame:
                    timer--;
                    enrageTimer--;
                    invisTimer--;       
                                      
                    if (enrageTimer > 0)
                    {                        
                        bossAnimation.playAnimation(gameTime);                       
                    }
                    else
                    {
                        flameTimer--;
                        enragedBossAnimation.playAnimation(gameTime);

                        foreach (Flame flame in flameList)
                        {
                            flame.Update(gameTime);
                            if (player.playerSize.Intersects(flame.flameSize) && invisTimer <= 0)
                            {
                                lives --;
                                invisTimer = 60 * 3;
                            }
                        }
                        for (int f = flameList.Count - 1; f >= 0; f--)
                        {
                            if (flame.position.Y < 0 + flameAnimation.Height)
                            {
                                flameList.Remove(flame);
                            }
                        }
                        if (flameTimer == 0)
                        {
                            int positionX = random.Next(0, 1280);
                            int positionY = 0;
                            flamePos = new Vector2(positionX, positionY);
                                           
                            int velocityY = random.Next(2, 4);
                            flameVelocity = new Vector2(0, velocityY);

                            flameList.Add(new Flame(flamePos, flameVelocity, flameAnimation, 100f, 10, true));
                            fireBall.Play(volume: 0.5f, pitch: 0.5f, pan: 0.0f);
                            flameTimer = 60 * 2;
                        }                        
                    }
                    princess.Update(gameTime);
                    
                    foreach (Enemy enemy in enemyList)
                    {
                        enemy.Update(gameTime, player); 
                        
                        if(player.playerSize.Contains(enemy.enemySize.X, enemy.enemySize.Y) && invisTimer <= 0)
                        {
                            lives--;
                            invisTimer = 60 * 5;
                            
                        }
                    }                                        
                    for (int e = enemyList.Count - 1; e >= 0; e--)
                    {
                       if (!enemyList[e].isAlive)
                       {
                          enemyList.Remove(enemyList[e]);
                            score = score + 10;
                            enemyDead.Play();
                       }
                    }                                                                                  
                    if (timer == 0)
                    {
                        randomSpeed = random.Next(60, 150);
                        enemyList.Add(new Enemy (Content, new Vector2(bridgeTexture.Width, bridgeTexture.Height * 3), randomSpeed, "Enemy-Skeleton-Sheet", 90f, 4, true));

                        timer = 60 * 4;
                    }                    
                    player.Update(gameTime);
                    if(player.playerSize.Contains(princess.princessSize.X, princess.princessSize.Y))
                    {
                        currentState = GameState.win;
                    }
                    else if (lives == 0)
                    {
                        currentState = GameState.gameOver;
                    }
                    break;

                case GameState.gameOver:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.start:
                    for (int i = 0; i < stringList.Count; i++)
                    {
                        for (int j = 0; j < stringList[i].Length; j++)
                        {
                            Vector2 drawTilePosStart = new Vector2(64 * j, 64 * i);
                            if (stringList[i][j] == 'w')
                            {
                                spriteBatch.Draw(bridgeTexture, drawTilePosStart, Color.White);
                            }
                            else if (stringList[i][j] == 'x')
                            {
                                spriteBatch.Draw(ladderTexture, drawTilePosStart, Color.White);
                            }
                            else if (stringList[i][j] == '#')
                            {
                                spriteBatch.Draw(ladderTexture, drawTilePosStart, Color.White);
                            }
                            else if (stringList[i][j] == 'o')
                            {
                                spriteBatch.Draw(wallTile, drawTilePosStart, Color.White);
                            }

                        }
                    }
                    spriteBatch.Draw(startText, new Vector2(350, 200), Color.White);
                    break;

                case GameState.inGame:

                    
                    if(enrageTimer > 0)
                    {
                        bossAnimation.drawBoss(spriteBatch);
                    }
                    else
                    {
                        enragedBossAnimation.drawBoss(spriteBatch);
                    }
                   
                    princess.Draw(spriteBatch);
                    for (int i = 0; i < stringList.Count; i++)
                    {
                        
                        for (int j = 0; j < stringList[i].Length; j++)
                        {
                            Vector2 drawTilePosInGame = new Vector2(64 * j, 64 * i);
                            if (stringList[i][j] == 'w')
                            {
                                spriteBatch.Draw(bridgeTexture, drawTilePosInGame, Color.White);
                            }
                            else if (stringList[i][j] == 'x')
                            {
                                spriteBatch.Draw(ladderTexture, drawTilePosInGame, Color.White);
                            }
                            else if (stringList[i][j] == '#')
                            {
                                spriteBatch.Draw(ladderTexture, drawTilePosInGame, Color.White);
                            }
                            else if (stringList[i][j] == 'o')
                            {
                                spriteBatch.Draw(wallTile, drawTilePosInGame, Color.White);
                            }                                               
                        }
                    }
                    foreach (Enemy enemy in enemyList)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    foreach (Flame flame in flameList)
                    {
                        flame.Draw(spriteBatch);
                    }
                    
                    player.Draw(spriteBatch);

                    DrawHearts(spriteBatch);
                    spriteBatch.DrawString(font, "YOUR CURRENT SCORE: " + score, new Vector2(510, 600), Color.White);
                    break;

                case GameState.gameOver:
                    spriteBatch.Draw(gameOver, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "YOUR SCORE WAS: " + score, new Vector2(480, 650), Color.White);
                    break;

                case GameState.win:
                    spriteBatch.Draw(win, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "YOUR SCORE WAS: " + score, new Vector2(480, 650), Color.White);
                    break;
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
        public static bool GetTileAtPosition(Vector2 vec)
        {
            return tileArray[(int)vec.X / 64, (int)vec.Y / 64].wall;
        }
        public static bool GetLadderAtPosition(Vector2 ladderVec)
        {                
            return tileArray[(int)ladderVec.X / 64, (int)ladderVec.Y / 64].ladder;
        }
        public static bool GetLadderAtPositionInvisible(Vector2 ladderVec)
        {
            return tileArray[(int)ladderVec.X / 64, (int)ladderVec.Y / 64].invisible;
        }
        public void DrawHearts(SpriteBatch spriteBatch)
        {
            if(lives == 5)
            {
                spriteBatch.Draw(heartTexture, new Vector2(500, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(550,620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(600,620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(650,620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(700,620), Color.White);
            }
            else if( lives == 4)
            {
                spriteBatch.Draw(heartTexture, new Vector2(500, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(550, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(600, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(650, 620), Color.White);                

            }
            else if(lives == 3)
            {
                spriteBatch.Draw(heartTexture, new Vector2(500, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(550, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(600, 620), Color.White);
               
            }
            else if(lives == 2)
            {
                spriteBatch.Draw(heartTexture, new Vector2(500, 620), Color.White);
                spriteBatch.Draw(heartTexture, new Vector2(550, 620), Color.White);                
            }
            else if(lives == 1)
            {
                spriteBatch.Draw(heartTexture, new Vector2(500, 620), Color.White);
               
            }
           
        }
        

    }
}