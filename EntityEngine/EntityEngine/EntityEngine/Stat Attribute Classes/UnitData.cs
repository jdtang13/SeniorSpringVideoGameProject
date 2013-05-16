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

        List<string> spells = new List<string>(new string[] { "Elfire", "Chronosphere", "Arcthunder" });
        List<string> attacks = new List<string>(new string[] { "Slash", "Blink Strike", "Blade Fury" });
        List<string> items = new List<string>(new string[] { "Potion", "Vulnerary", "Clarity" });

        public List<string> Spells() { return spells; }
        public List<string> Attacks() { return attacks; }
        public List<string> Items() { return items; }

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

        // this is the exp you gain after processing and adjustment based on your current level
        public void GainExp(int bounty)
        {
            AddExp(bounty); //  TODO: for now, no processing

            CheckAndLevelUp();
        }

        public int ExpRequiredForLevelUp()
        {
            return 100 * (int)(Math.Pow(2, level));
            // level up requirement goes from 100, 200, 400...
        }

        //  todo: right now, leveling up does nothing
        public void CheckAndLevelUp()
        {
            int expRequirement = ExpRequiredForLevelUp();
            while (exp >= expRequirement)
            {
                level++;
                exp -= expRequirement;

                expRequirement = ExpRequiredForLevelUp();
            }
        }

        // DON'T use this function. Use GainExp() for exp addition and processing.
        void AddExp(int myAdd)
        {
            exp += myAdd;
            //TODO: Check to see if you've leveled up
        }

        int currentHealth;
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        int maxHealth;
        public int GetMaxHealth()
        {
            return maxHealth;
        }
        public void AddToHealth(int diff) //  adds a value to the health safely, without exceeding bounds
        {
            currentHealth += diff;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            else if (currentHealth < 0) currentHealth = 0;
        }
        public void RemoveHealth(int diff)
        {
            AddToHealth(-diff);
        }

        int currentMana = 10;
        public int GetCurrentMana()
        {
            return currentMana;
        }
        int maxMana;
        public int GetMaxMana()
        {
            return maxMana;
        }
        
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

        Weapon equippedWeapon;
        public int PhysicalDamageAgainst(UnitData enemy)
        {
            int damage = 0;
            int weaponBonus = 0;
            if (equippedWeapon != null)
            {
                weaponBonus = equippedWeapon.Damage();
            }

            damage += weaponBonus + attributes["strength"] - enemy.attributes["defense"];

            return damage;
        }

        public int MagicalDamageAgainst(UnitData enemy)
        {
            int damage = 0;

            int weaponBonus = 0;
            if (equippedWeapon != null)
            {
                weaponBonus = equippedWeapon.Damage();
            }

            damage += weaponBonus + attributes["magic"] - enemy.attributes["resistance"];

            return damage;
        }

        //  the amount of raw exp you receive from beating this unit...this number can
        //  be processed and adjusted by the client based on exp splitting, exp bonuses,
        //  exp reduction due to high level, etc.
        public int ExpBounty()
        {
            int sum = 0;

            foreach (KeyValuePair<string, int> kvp in attributes)
            {
                int diff = kvp.Value;
                string attributeName = kvp.Key;
                
                float multiplier = 1.0f;

                if (attributeName == "strength" || attributeName == "magic") {
                    multiplier = 3.0f;
                }
                else if (attributeName == "resistance" || attributeName == "defense")
                {
                    multiplier = 2.5f;
                }
                else if (attributeName == "speed")
                {
                    multiplier = 2.0f;
                }

                sum += (int) (multiplier * diff);
            }

            return sum;
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

            Refresh();
        }

        public void Refresh()
        {
            movement += role.movement;
            sightRadius += role.sightRadius;
            attackRadius += role.attackRadius;

            maxHealth = attributes["strength"] * 2; currentHealth = maxHealth;
            maxMana = attributes["magic"] * 2; currentMana = maxMana;

            // TODO: for each item in this unit's inventory, if have the weapon
            //  in the highest row be auto-equipped as their current weapon.
        }


        ////currentHealth, mana, and movement calculated by adding the attributed based on the uninterpretedActors role with the TODO: pls finish this sentence
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
