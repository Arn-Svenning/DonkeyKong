using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using Microsoft.Xna.Framework.Content;

namespace DonkeyKong
{
    internal class Bullets
    {
        public Texture2D texture;

        public Vector2 velocity;
        public Vector2 velocityBackward;
        public Vector2 origin;
        public Vector2 position;

        public bool isVisible;

        public Bullets(Texture2D newTexture)
        {
            this.texture = newTexture;

            isVisible = false;

            origin = new Vector2(-50, -30);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
