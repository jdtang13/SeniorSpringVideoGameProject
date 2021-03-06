﻿using System;
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
    public class NestedMenu : Menu
    {
        int currentLayer = 0; //  which layer of options you're on. 0 is the default layer.
        Dictionary<string, List<string>> optionMenuMapper = new Dictionary<string, List<string>>();

        List<int> currentOptionIndices = new List<int>(new int[] { 0, 0 });
        int registeredOptionIndex = -1;

        public NestedMenu(bool isVisible)
        {
            this.isVisible = isVisible;
        }

        public NestedMenu(List<string> options, int width, int height, int x, int y, Color color, Texture2D texture, SpriteFont font)
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
            currentLayer = 0;

            isVisible = true;
        }

        //  adds a new options menu as suboptions of an existing options.
        //  e.g., if "Fire" is a suboption of "Cast Spell", then you could do
        //  AddNestedOptions("Cast Spell", <options ("Fire", "Thunder")>);
        public void AddNestedOptions(string oldOption, List<string> newOptions)
        {
            optionMenuMapper.Add(oldOption, newOptions);
        }

        //  when the user clicks "enter", remember which option they entered in through
        public void RegisterOption()
        {
            registeredOptionIndex = currentOptionIndices[currentLayer];
        }

        public string RegisteredOption() {

            if (registeredOptionIndex == -1) return "";

            return options[registeredOptionIndex];
        }

        public override void SetSelectedOption(int index)
        {
            while (currentLayer >= currentOptionIndices.Count)
            {
                currentOptionIndices.Add(0);
            }
            currentOptionIndices[currentLayer] = index;
        }

        public override int CurrentOptionIndex()
        {
            return currentOptionIndices[currentLayer];
        }

        public override string CurrentOption()
        {
            if (currentLayer == 0)
            {
                return options[currentOptionIndices[currentLayer]];
            }
            return optionMenuMapper[options[registeredOptionIndex]][currentOptionIndices[currentLayer]];
        }

        public void ScrollUp()
        {
            //  read from normal options on the 0 layer
            if (currentLayer == 0)
            {
                SetSelectedOption((options.Count + CurrentOptionIndex() - 1) % options.Count);
            }
            //  read from nested options on the >0 layers
            else if (optionMenuMapper.ContainsKey(options[registeredOptionIndex])) //  check to see if it has any nested options
            {
                SetSelectedOption((optionMenuMapper[options[registeredOptionIndex]].Count + CurrentOptionIndex() - 1) % optionMenuMapper[RegisteredOption()].Count);
            }
        }

        public void ScrollDown()
        {
            if (currentLayer == 0)
            {
                SetSelectedOption((CurrentOptionIndex() + 1) % options.Count);
            }
            else if (optionMenuMapper.ContainsKey(options[registeredOptionIndex]))
            {
                SetSelectedOption((CurrentOptionIndex() + 1) % optionMenuMapper[RegisteredOption()].Count);
            }
        }

        public void SetLayer(int layer)
        {
            currentLayer = layer;
        }

        public int Layer() { return currentLayer; }

        public void Enter()
        {
            RegisterOption();
            currentLayer++;
        }

        public void Back()
        {
            currentLayer = 0;
            currentOptionIndices[1] = 0;
            registeredOptionIndex = -1;
        }

        public void Leave()
        {
            if (currentLayer != 0)
            {
                currentLayer--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {

            if (options != null && isVisible)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    Color tmp = menuOptionColor;
                    if (i == currentOptionIndices[0])
                    {
                        tmp = Color.Black;
                    }

                    spriteBatch.Draw(menuOptionTexture, new Rectangle(x, y + i * menuOptionHeight, menuOptionWidth, menuOptionHeight), tmp);
                    spriteBatch.DrawString(font, options[i], new Vector2(x + .05f * menuOptionWidth, y + i * menuOptionHeight), Color.White);
                }

                if (registeredOptionIndex != -1 && (optionMenuMapper.ContainsKey(options[registeredOptionIndex])))
                {
                    for (int i = 0; i < optionMenuMapper[options[registeredOptionIndex]].Count; i++)
                    {
                        Color tmp = menuOptionColor;
                        if (i == currentOptionIndices[currentLayer])
                        {
                            tmp = Color.Black;
                        }

                        spriteBatch.Draw(menuOptionTexture, new Rectangle(x + menuOptionWidth, y + i * menuOptionHeight + registeredOptionIndex*menuOptionHeight, menuOptionWidth, menuOptionHeight), tmp);
                        spriteBatch.DrawString(font, optionMenuMapper[options[registeredOptionIndex]][i], new Vector2(x + .05f * menuOptionWidth + menuOptionWidth, y + i * menuOptionHeight + registeredOptionIndex*menuOptionHeight), Color.White);
                    }
                }
            }

        }
    }
}
