using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine
{
    public class Role  //attributes growths and caps for the roll of characters
    {
        public Dictionary<string, int> attributes = new Dictionary<string, int>();
        public Dictionary<string, float> growths = new Dictionary<string, float>();
        public Dictionary<string, int> caps = new Dictionary<string, int>();

        public string weapon;

        //TODO: Calculate these if they need to be calc'd
        //Aren't these unitdata specific and not class specific?
        //public int health;
        //public int mana;
        public int movement;

        public int sightRadius;
        public int attackRadius;

        public Role(
                    int str, int mag, int dex, int agi, int def, int res, int spd,
                    float strGrowth, float magGrowth, float dexGrowth, float agiGrowth, float defGrowth, float resGrowth, float spdGrowth,
                    int strCap, int magCap, int dexCap, int agiCap, int defCap, int resCap, int spdCap,
                    string weapon,
                    bool light, bool anima, bool dark,
                    int movement, int sightRange, int attackRange)
        {
            this.movement = movement;
            this.sightRadius = sightRange;
            this.attackRadius = attackRange;
            this.weapon = weapon;

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

            //weapons["sword"] = sword;
            //weapons["lance"] = lance;
            //weapons["axe"] = axe;
            //weapons["light"] = light;
            //weapons["anima"] = anima;
            //weapons["dark"] = dark;
            //weapons["bow"] = bow;
        }

    }
}
