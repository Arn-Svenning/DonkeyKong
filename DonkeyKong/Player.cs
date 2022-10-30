using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DonkeyKong
{
    public class Player
    {
        SoundEffect gun;
        Vector2 position;

        Vector2 destination;
        Vector2 direction;
        float speed = 100.0f;
        bool moving = false;

        /// <playerAnimation>        
        Texture2D animation;
        Rectangle sourceRectangle;
        public Rectangle playerSize;

        float elapsedTime;
        float frameTime;
        int numberOfFrames;
        int currentFrame;
        int frameWidth;
        int frameHeight;
        bool looping;

        bool pressed = false;
        bool front;
        bool back;
                
        List<Bullets> bulletList = new List<Bullets>();
        Texture2D bulletTexture;
        public Rectangle bulletSize;
        public Player (ContentManager Content, Vector2 position, string asset, float frameSpeed, int numberOfFrames, bool looping)
        {
            
            this.position = position;

            this.frameTime = frameSpeed;
            this.numberOfFrames = numberOfFrames;
            this.looping = looping;
            this.animation = Content.Load<Texture2D>(asset);
            frameWidth = (animation.Width / numberOfFrames);
            frameHeight = (animation.Height);

            bulletTexture = Content.Load<Texture2D>("bullet");

            gun = Content.Load<SoundEffect>("Shotgun");
        }
        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            playerSize = new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);                                                                          
            
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                shoot();
            }

            UpdateBullets(gameTime);
            if (!moving)
            {
                
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    ChangeDirection(new Vector2(-1, 0));                    

                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    ChangeDirection(new Vector2(1, 0));

                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    ChangeDirection(new Vector2(0, -1));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    ChangeDirection(new Vector2(0, 1));
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
                }
            }
            if (moving == true)
            {
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
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(bulletTexture, playerSize, Color.Red);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                spriteBatch.Draw(animation, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
            }            
            else
            {
                spriteBatch.Draw(animation, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
                
            foreach (Bullets bullet in bulletList)
            {
                bullet.Draw(spriteBatch);
            }

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
        }
        public void UpdateBullets(GameTime gameTime)
        {
            
            foreach (Bullets bullet in bulletList)
            {
                bulletSize = new Rectangle((int)bullet.position.X, (int)bullet.position.Y, bulletTexture.Width, bulletTexture.Height);

                if (Keyboard.GetState().IsKeyDown(Keys.Left) && pressed == false)
                {                    
                    pressed = true;
                    back = true;
                    gun.Play();                   
                }
                else if (pressed == true && back == true)
                {
                    bullet.position += bullet.velocityBackward;                       
                    if (Vector2.Distance(bullet.position, position) > 1500)
                    {
                        pressed = false;
                        back = false;
                        bullet.isVisible = false;                                               
                    }
                }
                else if (pressed == false)
                {
                    pressed = true;
                    front = true;
                    gun.Play();
                }
                else if (pressed == true && front == true)
                {
                    bullet.position += bullet.velocity;
                    if (Vector2.Distance(bullet.position, position) > 1500)
                    {
                        pressed = false;
                        front = false;
                        bullet.isVisible = false;                        
                    }
                }                                                
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (!bulletList[i].isVisible)
                {
                    bulletList.RemoveAt(i);
                    i--;                  
                }
            }
        }
        public void shoot()
        {
            Bullets newBullet = new Bullets(bulletTexture);

            newBullet.velocity = new Vector2(4, 0);
            newBullet.velocityBackward = new Vector2(-4, 0);
            newBullet.position = position + newBullet.velocity;
            newBullet.position = position + newBullet.velocityBackward;

            newBullet.isVisible = true;
            if (bulletList.Count < 1)
            {
                bulletList.Add(newBullet);
            }
        }
    }
}
