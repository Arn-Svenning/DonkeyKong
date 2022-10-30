using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DonkeyKong
{
    public class Enemy
    {
        public Vector2 position;

        public Vector2 destination;
        public Vector2 direction;
        public int speed;
        public bool moving = false;
        public bool movingDown = false;
        public bool movingUp = false;
       
        /// <playerAnimation>        
        public Texture2D animation;
        public Rectangle sourceRectangle;
        public Rectangle enemySize;

        public float elapsedTime;
        public float frameTime;
        public int numberOfFrames;
        public int currentFrame;
        public int frameWidth;
        public int frameHeight;
        bool looping;
        public bool isKilled;        
        public bool isAlive;        

        enum MoveState { movingLeft, movingRight, movingUp, movingDown }
        MoveState currentState = MoveState.movingRight;

        public Enemy(ContentManager Content, Vector2 position, int speed, string asset, float frameSpeed, int numberOfFrames, bool looping)
        {

            this.position = position;
            this.speed = speed;
            this.frameTime = frameSpeed;
            this.numberOfFrames = numberOfFrames;
            this.looping = looping;
            this.animation = Content.Load<Texture2D>(asset);
            frameWidth = (animation.Width / numberOfFrames);
            frameHeight = (animation.Height);            

            isAlive = true;
        }
        public void Update(GameTime gameTime, Player player)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            enemySize = new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);

            Vector2 newDestination = position + direction * 64.0f;                        

            if (elapsedTime >= frameTime)
            {
                if (currentFrame >= numberOfFrames - 1)
                {
                    currentFrame = 0;

                }
                else
                {
                    currentFrame++;
                }
                elapsedTime = 0;
            }
            if (!moving)
            {
                switch (currentState)
                {
                    case MoveState.movingLeft:
                        ChangeDirection(new Vector2(-1, 0));
                        if (position == destination)
                        {
                            currentState = MoveState.movingRight;

                        }
                        else if (!movingDown && Game1.GetLadderAtPosition(destination))
                        {
                            currentState = MoveState.movingDown;
                        }

                        break;

                    case MoveState.movingRight:
                        ChangeDirection(new Vector2(1, 0));
                        if (position == destination)
                        {
                            currentState = MoveState.movingLeft;
                        }
                        else if (!movingUp && Game1.GetLadderAtPositionInvisible(destination))
                        {
                            currentState = MoveState.movingUp;
                        }

                        break;

                    case MoveState.movingUp:
                        ChangeDirection(new Vector2(0, -1));
                        if (position == destination)
                        {
                            currentState = MoveState.movingLeft;
                        }
                        break;

                    case MoveState.movingDown:
                        ChangeDirection(new Vector2(0, 1));
                        if (position == destination)
                        {
                            currentState = MoveState.movingRight;
                        }

                        break;
                }

            }
            else
            {
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Check if we are near enough to the destination
                if (Vector2.Distance(position, destination) < 1)
                {
                    position = destination;
                    moving = false;
                    movingDown = false;
                    movingUp = false;
                }
            }

            if (enemySize.Contains(player.bulletSize.X, player.bulletSize.Y))
            {
                isKilled = IsKilled(player.bulletSize.X, player.bulletSize.Y);
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {

           //spriteBatch.Draw(animation, enemySize, Color.Red);
           spriteBatch.Draw(animation, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            
            
        }
        public void ChangeDirection(Vector2 dir)
        {
            direction = dir;
            Vector2 newDestination = position + direction * 64.0f;            

            //Check if we can move in the desired direction, if not, do nothing
            if (!Game1.GetTileAtPosition(newDestination))
            {
                destination = newDestination;
                moving = true;
            }
            else if(Game1.GetLadderAtPosition(newDestination))
            {
                destination = newDestination;
                movingDown = true;
               
            }
            else if(Game1.GetLadderAtPositionInvisible(newDestination))
            {
                destination = newDestination;
                movingUp = true;
            }
          
        }
        public bool IsKilled(int x, int y)
        {
            bool isKilled = false;
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, animation.Width, animation.Height);

            if (rect.Contains(x, y))
            {
                isKilled = true;
                isAlive = false;
            }
            return isKilled;


        }
    }           
}
