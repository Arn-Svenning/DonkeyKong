using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DonkeyKong
{
    internal class Flame
    {
        public Vector2 position;
        public Vector2 velocity;
        public Texture2D flame;

        public Rectangle flameSize;

        public bool alive;

        public Rectangle sourceRectangle;
       
        public float elapsedTime;
        public float frameTime;
        public int numberOfFrames;
        public int currentFrame;
        public int width;
        public int height;
        public int frameWidth;
        public int frameHeight;
        public bool looping;


        public Flame(Vector2 position, Vector2 velocity, Texture2D flame, float frameSpeed, int numberOfFrames, bool looping)
        {
            this.position = position;
            this.velocity = velocity;
            this.flame = flame;
            this.frameTime = frameSpeed;
            this.numberOfFrames = numberOfFrames;
            this.looping = looping;
            frameWidth = (flame.Width / numberOfFrames);
            frameHeight = (flame.Height);
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            /////Looping the spriteSheet
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

            position = position + velocity;
            flameSize = new Rectangle((int)position.X, (int)position.Y, frameWidth - 100, frameHeight - 70);
        }
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(flame, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);

            //spriteBatch.Draw(flame, flameSize, Color.Pink);
        }
    }
}
