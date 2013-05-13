using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Stat_Attribute_Classes
{
    class Armor
    {
        StatisticalBuff buff;

        int durability;
        int armorRank;

        public Armor (int health, int mana, int avoid, int crit, int movementBuff, int durability, int armorRank,
            int str, int mag, int dex, int agi, int def, int res)
        {
            buff = new StatisticalBuff(health, mana, avoid, crit, movementBuff);

            Dictionary<string, int> attributes = buff.Attributes();

            attributes["strength"] = str;
            attributes["magic"] = mag;
            attributes["dexterity"] = dex;
            attributes["agility"] = agi;
            attributes["defense"] = def;
            attributes["resistance"] = res;
        }
    }
}
