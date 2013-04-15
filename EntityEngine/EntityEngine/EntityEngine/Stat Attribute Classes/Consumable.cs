using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine
{
    class Consumable
    {
        public Dictionary<string, int> consumableAttributes;

        public Consumable(int manamod, int spdmod, int heal, int manareg)
        {
            consumableAttributes = new Dictionary<string, int>();

            consumableAttributes["mana modifier"] = manamod;
            consumableAttributes["speed modifier"] = spdmod;
            consumableAttributes["heal"] = heal;
            consumableAttributes["mana regen"] = manareg;
        }
    }
}
