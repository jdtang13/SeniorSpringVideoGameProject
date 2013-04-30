using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Stat_Attribute_Classes;

namespace EntityEngine.Stat_Attribute_Classes
{
    public class UnitData : StatisticalEntity
    {
        string unitName;
        public string GetName()
        {
            return unitName;
        }

        Alignment alignment;
        public Alignment GetAlignment()
        {
            return alignment;
        }
        public void SetAlignment(Alignment myAli)
        {
            alignment = myAli;
        }

        Role role;
        public Role GetRole()
        {
            return role;
        }

        List<string> knownSpells = new List<string>();
        List<string> knownAttacks = new List<string>();

        int level;
        public int GetCurrentLevel()
        {
            return level;
        }
        public void LevelUp()
        {
            level++;
        }

        int exp = 0;
        public int GetCurrentExp()
        {
            return exp;
        }
        public void AddExp(int myAdd)
        {
            exp += myAdd;
            //TODO: Check to see if you've leveled up
        }

        int currentHealth;
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        int totalHealth;
        public int GetTotalHealth()
        {
            return totalHealth;
        }

        int currentMana = 10;
        public int GetCurrentMana()
        {
            return currentMana;
        }
        int totalMana;
        public int GetTotalMana()
        {
            return totalMana;
        }
        
        int experienceBounty; // exp dropped when this unit dies //Maybe like 1/4 of total exp?

        int movement;
        public int GetMovement()
        {
            return movement;
        }

        int sightRadius;
        public int GetSightRadius()
        {
            return sightRadius;
        }
        public void SetSightRadius(int myRadius)
        {
            sightRadius = myRadius;
        }

        int attackRadius;
        public int GetAttackRadius()
        {
            return attackRadius;
        }

        public UnitData(string name,     Role role,       Alignment ali,   int level, 
                        int str,         int mag,         int dex,         int agi,         int def,         int res,         int spd,
                        float strGrowth, float magGrowth, float dexGrowth, float agiGrowth, float defGrowth, float resGrowth, float spdGrowth,
                        int strCap,      int magCap,      int dexCap,      int agiCap,      int defCap,      int resCap,      int spdCap,
                        int movement,    int sightRange,  int attackRange)
        {
            this.unitName = name;
            this.role = role;
            this.alignment = ali;
            this.level = level;

            attributes = new Dictionary<string, int>();
            caps = new Dictionary<string, int>();
            growths = new Dictionary<string, float>();

            this.level = level;
            this.movement = movement;
            this.sightRadius = sightRange;
            this.attackRadius = attackRange;

            attributes["strength"] = str + role.Attributes()["strength"];
            attributes["magic"] = mag + role.Attributes()["magic"];
            attributes["dexterity"] = dex + role.Attributes()["dexterity"];
            attributes["agility"] = agi + role.Attributes()["agility"];
            attributes["defense"] = def + role.Attributes()["defense"];
            attributes["resistance"] = res + role.Attributes()["resistance"];
            attributes["speed"] = spd + role.Attributes()["speed"];

            caps["strength"] = strCap + role.Caps()["strength"];
            caps["magic"] = magCap + role.Caps()["magic"];
            caps["dexterity"] = dexCap + role.Caps()["dexterity"];
            caps["agility"] = agiCap + role.Caps()["agility"];
            caps["defense"] = defCap + role.Caps()["defense"];
            caps["resistance"] = resCap + role.Caps()["resistance"];
            caps["speed"] = spdCap + role.Caps()["speed"];

            growths["strength"] = strGrowth + role.Growths()["strength"];
            growths["magic"] = magGrowth + role.Growths()["magic"];
            growths["dexterity"] = dexGrowth + role.Growths()["dexterity"];
            growths["agility"] = agiGrowth + role.Growths()["agility"];
            growths["defense"] = defGrowth + role.Growths()["defense"];
            growths["resistance"] = resGrowth + role.Growths()["resistance"];
            growths["speed"] = spdGrowth + role.Growths()["speed"];

            CalculateValues();
        }

        public void CalculateValues()
        {
            movement += role.movement;
            sightRadius += role.sightRadius;
            attackRadius += role.attackRadius;

            totalHealth = attributes["strength"] * 2; currentHealth = totalHealth;
            totalMana = attributes["magic"] * 2; currentMana = totalMana;
        }


        ////currentHealth, mana, and movement calculated by adding the attributed based on the characters role with the TODO: pls finish this sentence
        //public int Health()
        //{

        //    return 2 * attributes["strength"] + role.health;
        //}

        //public int Movement()
        //{
        //    return movement + role.movement;   
        //}
        //public int Mana()
        //{
        //    //  mana = 2 * magic
        //    return 2*attributes["magic"] + role.mana;
        //}

        //public int Strength()
        //{
        //    return attributes["strength"] + role.attributes["strength"];
        //}

        //public Dictionary<string, int> Attributes()
        //{
        //    Dictionary<string, int> tmp = new Dictionary<string, int>();

        //    foreach (KeyValuePair<string, int> kvp in attributes)
        //    {
        //        tmp[kvp.Key] = attributes[kvp.Key] + role.attributes[kvp.Key];
        //    }

        //    return tmp;
        //}

        //public Dictionary<string, float> Growths()
        //{
        //    Dictionary<string, float> tmp = new Dictionary<string, float>();

        //    foreach (KeyValuePair<string, float> kvp in growths)
        //    {
        //        tmp[kvp.Key] = growths[kvp.Key] + role.growths[kvp.Key];
        //    }

        //    return tmp;
        //}

        //public Dictionary<string, int> Caps()
        //{
        //    Dictionary<string, int> tmp = new Dictionary<string, int>();

        //    foreach (KeyValuePair<string, int> kvp in caps)
        //    {
        //        tmp[kvp.Key] = caps[kvp.Key] + role.caps[kvp.Key];
        //    }

        //    return tmp;
        //}

        // ....

        //int wprof = 10;

        /* add item drop
         * add sprite
         * */
    }
}
