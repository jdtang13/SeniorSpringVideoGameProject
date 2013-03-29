using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorProjectGame
{
    class Armor
    {
        public Dictionary<string, int> armorAttributes;

        public Armor (int hlthmod, int manamod, int drb, int wprof, int wprofr,
            int movemod, int strmod, int dexmod, int agimod, int defmod, int resmod, int spdmod)
        {
            armorAttributes = new Dictionary<string, int>();
            armorAttributes["health modifier"] = hlthmod;
            armorAttributes["mana modifier"] = manamod;
            armorAttributes["durability"] = drb;
            armorAttributes["armor proficiency requirement"] = wprofr;
            armorAttributes["move modifier"] = movemod;
            armorAttributes["strength modifier"] = strmod;
            armorAttributes["dexterity modifier"] = dexmod;
            armorAttributes["agility modifier"] = agimod;
            armorAttributes["defense modifier"] = defmod;
            armorAttributes["resistance modifier"] = resmod;
            armorAttributes["speed modifier"] = spdmod;

            //hi test
        }
    }
}
