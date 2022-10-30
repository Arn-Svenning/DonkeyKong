using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace DonkeyKong
{
    public class Tiles
    {
        public Texture2D tex;
        public Vector2 pos;
        public bool wall;
        public bool ladder;
        public bool invisible;
        public Rectangle rect;

        public Tiles(Texture2D tex, Vector2 pos, bool wall, bool ladder, bool invisible)
        {
            this.tex = tex;
            this.pos = pos;
            this.wall = wall;
            this.ladder = ladder;
            this.invisible = invisible;

            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, rect, Color.White);
        }
    }
}
