using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine
{
    public class Role  //attributes growths and caps for the roll of characters
    {
        public Dictionary<string, int> attributes;

        public Dictionary<string, bool> weapons;
        public Dictionary<string, float> growths;

        public Dictionary<string, int> caps;

        public int health;
        public int mana;
        public int movement;

        public Role(int str, int mag, int dex, int agi, int def, int res, int spd,
             float strGrowth, float magGrowth, float dexGrowth, float agiGrowth, float defGrowth, float resGrowth, float spdGrowth,
             int strCap, int magCap, int dexCap, int agiCap, int defCap, int resCap, int spdCap,
                    bool sword, bool lance, bool axe, bool light, bool anima, bool dark, bool bow, int movement)
        {
            attributes = new Dictionary<string, int>();
            growths = new Dictionary<string, float>();
            caps = new Dictionary<string, int>();
            weapons = new Dictionary<string, bool>();

            this.movement = movement;

            attributes["strength"] = str;
            attributes["magic"] = mag;
            attributes["dexterity"] = dex;
            attributes["agility"] = agi;
            attributes["defense"] = def;
            attributes["resistance"] = res;
            attributes["speed"] = spd;

            growths["strength"] = strGrowth;
            growths["magic"] = magGrowth;
            growths["dexterity"] = dexGrowth;
            growths["agility"] = agiGrowth;
            growths["defense"] = defGrowth;
            growths["resistance"] = resGrowth;
            growths["speed"] = spdGrowth;

            caps["strength"] = strCap;
            caps["magic"] = magCap;
            caps["dexterity"] = dexCap;
            caps["agility"] = agiCap;
            caps["defense"] = defCap;
            caps["resistance"] = resCap;
            caps["speed"] = spdCap;

            weapons["sword"] = sword;
            weapons["lance"] = lance;
            weapons["axe"] = axe;
            weapons["light"] = light;
            weapons["anima"] = anima;
            weapons["dark"] = dark;
            weapons["bow"] = bow;
        }

    }
}
