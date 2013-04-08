using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorProjectGame
{
    public enum Alignment
    {
        PLAYER, ENEMY, NEUTRAL       // declares what allighments this unit can be 
    }

    public class Unit
    {
        string name;
        Alignment alignment = Alignment.ENEMY;
        //setting up consturctor for dictionary to hold character growths, attributs, and stats 
        Dictionary<string, int> attributes; // stats for units
        Dictionary<string, float> growths; // growth of attributes with each level
        Dictionary<string, int> caps;  // attribute cap for units

        int level;// = 1;

        //int health = 20;
        //int mana = 10;
        int exp = 0;
        int movement;// = 2;

        Role role;
        
        int experienceBounty; // exp dropped when this unit dies
        int sightRange;  // how far a character can see in squares
        int attackRange; // the range a character can attack


        public Unit(int str, int mag, int dex, int agi, int def, int res, int spd,
            float strGrowth, float magGrowth, float dexGrowth, float agiGrowth, float defGrowth, float resGrowth, float spdGrowth,
            int strCap, int magCap, int dexCap, int agiCap, int defCap, int resCap, int spdCap,
            string name, Alignment ali, Role role, int level, int movement)
        {
            attributes = new Dictionary<string, int>();

            this.level = level;
            this.movement = movement;

            attributes["strength"] = str;
            attributes["magic"] = mag; 
            attributes["dexterity"] = dex;
            attributes["agility"] = agi;
            attributes["defense"] = def;
            attributes["resistance"] = res;
            attributes["speed"] = spd;

            caps["strength"] = strCap;
            caps["magic"] = magCap;
            caps["dexterity"] = dexCap;
            caps["agility"] = agiCap;
            caps["defense"] = defCap;
            caps["resistance"] = resCap;
            caps["speed"] = spdCap;

            growths["strength"] = strGrowth;
            growths["magic"] = magGrowth;
            growths["dexterity"] = dexGrowth;
            growths["agility"] = agiGrowth;
            growths["defense"] = defGrowth;
            growths["resistance"] = resGrowth;
            growths["speed"] = spdGrowth;

            alignment = ali;
        }


        //health, manna, and movement calculated by adding the attributed based on the characters role with the 
        public int Health()
        {
            // health = 2 * strength

            return 2*attributes["strength"] + role.health;   
        }

        public int Movement()
        {
            return movement + role.movement;   
        }
        public int Mana()
        {
            //  mana = 2 * magic
            return 2*attributes["magic"] + role.mana;
        }

        public int Strength()
        {
            return attributes["strength"] + role.attributes["strength"];
        }

        public Dictionary<string, int> Attributes()
        {
            Dictionary<string, int> tmp = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> kvp in attributes)
            {
                tmp[kvp.Key] = attributes[kvp.Key] + role.attributes[kvp.Key];
            }

            return tmp;
        }

        public Dictionary<string, float> Growths()
        {
            Dictionary<string, float> tmp = new Dictionary<string, float>();

            foreach (KeyValuePair<string, float> kvp in growths)
            {
                tmp[kvp.Key] = growths[kvp.Key] + role.growths[kvp.Key];
            }

            return tmp;
        }

        public Dictionary<string, int> Caps()
        {
            Dictionary<string, int> tmp = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> kvp in caps)
            {
                tmp[kvp.Key] = caps[kvp.Key] + role.caps[kvp.Key];
            }

            return tmp;
        }

        // ....

        //int wprof = 10;

        /* add item drop
         * add sprite
         * */
    }
}
