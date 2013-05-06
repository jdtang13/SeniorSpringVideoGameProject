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
    public class NestedMenu : Menu
    {
        int currentLayer = 0; //  which layer of options you're on. 0 is the default layer.
        Dictionary<string, List<string>> optionMenuMapper = new Dictionary<string, List<string>>();

        List<int> currentOptionIndices = new List<int>(new int[] { -1, -1 });
        string registeredOption = "";

        public NestedMenu(bool isVisible)
        {
            this.isVisible = isVisible;
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
            registeredOption = this.CurrentOption();
        }

        public string RegisteredOption() { return registeredOption; }

        public override void SetSelectedOption(int index)
        {
            while (currentLayer > currentOptionIndices.Count)
            {
                currentOptionIndices.Add(-1);
            }
            currentOptionIndices[currentLayer] = index;
        }

        public override int CurrentOptionIndex()
        {
            return currentOptionIndices[currentLayer];
        }

        public override string CurrentOption()
        {
            return options[currentOptionIndices[currentLayer]];
        }

        public void ScrollUp()
        {
            if (currentLayer == 0)
            {
                SetSelectedOption((options.Count + CurrentOptionIndex() - 1) % options.Count);
            }
            else
            {
                SetSelectedOption((optionMenuMapper[registeredOption].Count + CurrentOptionIndex() - 1) % optionMenuMapper[registeredOption].Count);
            }
        }

        public void ScrollDown()
        {
            if (currentLayer == 0)
            {
                SetSelectedOption((CurrentOptionIndex() + 1) % options.Count);
            }
            else
            {
                SetSelectedOption((CurrentOptionIndex() + 1) % optionMenuMapper[registeredOption].Count);
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

        public void Leave()
        {
            if (currentLayer != 0)
            {
                currentLayer--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {

        }
    }
}
