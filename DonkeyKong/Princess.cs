using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonkeyKong
{
    internal class Princess
    {
        Texture2D animation;
        Rectangle sourceRectangle;
        public Rectangle princessSize;
        Vector2 pos;
                
        float elapsedTime;
        float frameTime;
        int numberOfFrames;
        int currentFrame;
        int width;
        int height;
        int frameWidth;
        int frameHeight;
        bool looping;

        public Princess(ContentManager Content, string asset, Vector2 pos, float frameSpeed, int numberOfFrames, bool looping)
        {
            this.frameTime = frameSpeed;
            this.numberOfFrames = numberOfFrames;
            this.looping = looping;
            this.animation = Content.Load<Texture2D>(asset);
            this.pos = pos;
            frameWidth = (animation.Width / numberOfFrames);
            frameHeight = (animation.Height);
        }
        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            princessSize = new Rectangle((int)pos.X, (int)pos.Y, frameWidth, frameHeight);

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
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(animation, pos, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);            
        }
    }
}
