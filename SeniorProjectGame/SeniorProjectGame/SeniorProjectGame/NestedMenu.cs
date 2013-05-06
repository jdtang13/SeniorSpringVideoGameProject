using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorProjectGame
{
    class NestedMenu : Menu
    {
        int currentLayer; //  which layer of options you're on. 0 is the default layer.
        Dictionary<string, Menu> optionMenuMapper = new Dictionary<string, Menu>();

        //  adds a new options menu as suboptions of an existing options.
        //  e.g., if "Fire" is a suboption of "Cast Spell", then you could do
        //  AddNestedOptions("Cast Spell", <Menu with options ("Fire", "Thunder")>);
        public void AddNestedOptions(string oldOption, Menu newOptions)
        {
            optionMenuMapper.Add(oldOption, newOptions);
        }
    }
}
