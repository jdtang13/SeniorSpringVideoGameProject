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

        bool isVisible;

        SpriteFont font;

        int x;
        int y;

        int menuOptionWidth;
        int menuOptionHeight;

        Color menuOptionColor;
        Texture2D menuOptionTexture;

        public Menu(bool visible)
        {
            isVisible = visible;
        }

        public List<ClickableComponent> Hitboxes()
        {
            return hitboxes;
        }

        public List<string> Options()
        {
            return options;
        }

        public void SetSelectedOption(int index)
        {
            currentOptionIndex = index;
        }

        public int CurrentOptionIndex()
        {
            return currentOptionIndex;
        }

        public Menu(List<string> options, int width, int height, int x, int y, Color color, Texture2D texture, SpriteFont font)
        {
            LoadOptions(options);
            menuOptionWidth = width;
            menuOptionHeight = height;
            menuOptionColor = color;

            menuOptionTexture = texture;

            this.x = x;
            this.y = y;

            this.font = font;

            currentOptionIndex = 0;

            isVisible = true;

            // to set texture, you can do:
            /*
             Texture2D dot = new Texture2D(graphics.GraphicsDevice, 1, 1);
             dot.SetData(new Color[] { Color.White });
             */
        }

        public void Show() { isVisible = true; }
        public void Hide() { isVisible = false; }

        void LoadOptions(List<string> options)
        {
            this.options = options;
            hitboxes = new List<ClickableComponent>();

            for (int i = 0; i < options.Count; i++)
            {
                // TODO: clickable component is buggy
                hitboxes.Add(new ClickableComponent(new Vector2(position.X, position.Y + menuOptionHeight*i),
                    menuOptionWidth, menuOptionHeight));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (options != null && isVisible)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    Color tmp = menuOptionColor;
                    if (i == currentOptionIndex)
                    {
                        tmp = Color.Black;
                    }

                    spriteBatch.Draw(menuOptionTexture, new Rectangle(x, y + i * menuOptionHeight, menuOptionWidth, menuOptionHeight), tmp);
                    spriteBatch.DrawString(font, options[i], new Vector2(x + .05f*menuOptionWidth, y + i * menuOptionHeight), Color.White);
                }
            }
        }

    }
}
