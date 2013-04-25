using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EntityEngine.Components.World_Map;

namespace SeniorProjectGame
{
    class Menu
    {
        Vector2 position;
        List<string> options;
        List<ClickableComponent> hitboxes;

        int currentOptionIndex;

        int menuOptionWidth;
        int menuOptionHeight;

        Color menuOptionColor;
        Texture2D menuOptionTexture;

        public Menu(List<string> options, int width, int height, Color color, Texture2D texture)
        {
            LoadOptions(options);
            menuOptionWidth = width;
            menuOptionHeight = height;
            menuOptionColor = color;

            menuOptionTexture = texture; 
            // to set texture, you can do:
            /*
             Texture2D dot = new Texture2D(graphics.GraphicsDevice, 1, 1);
             dot.SetData(new Color[] { Color.White });
             */
        }

        void LoadOptions(List<string> options)
        {
            this.options = options;
            for (int i = 0; i < options.Count; i++)
            {
                hitboxes.Add(new ClickableComponent(new Vector2(position.X + menuOptionWidth/2, position.Y + menuOptionHeight/2 + menuOptionHeight*i),
                    menuOptionWidth, menuOptionHeight));
            }
        }

        void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
