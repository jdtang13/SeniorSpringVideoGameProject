using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EntityEngine.Stat_Attribute_Classes
{
    class Mount : StatisticalEntity {

        int movement;

        public Mount(int str, int mag, int dex, int agi, int def, int res, int movement)
        {
            attributes = new Dictionary<string, int>();

            attributes["strength"] = str;
            attributes["magic"] = mag;
            attributes["dexterity"] = dex;
            attributes["agility"] = agi;
            attributes["defense"] = def;
            attributes["resistance"] = res;

            this.movement = movement;
        }
    }
}
