using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorProjectGame
{
    class Scroll
    {
        public Dictionary<string, int> attributes;

        public Scroll(int manamod, int magmod, int agimod, int resmod, int spdmod)
        {
            attributes = new Dictionary<string, int>();
            
            attributes["mana modifier"] = manamod;
            attributes["magic modifier"] = magmod;
            attributes["agility modifier"] = agimod;
            attributes["resistance modifier"] = resmod;
            attributes["speed modifier"] = spdmod;

        }
    }
}
