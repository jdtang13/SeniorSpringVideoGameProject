using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Stat_Attribute_Classes;

namespace EntityEngine
{
    class Weapon
    {
        StatisticalBuff buff; // statistical changes that result from wielding a weapon,
                              // e.g. Sword of Strength gives you +2 strength

        int damage;
        int accuracy; // chance of a weapon hitting correctly
        int critChance;
        int guardChance;
        int durability; //  how much a weapon can be used until it breaks

        int weaponRank; //  0 is the lowest tier of weapons, and as it increases it's more advanced

       public Weapon(int damage, int accuracy, int critChance, int guardChance, int durability,
           int weaponRank, int str, int mag, int dex, int agi, int def, int res,
           int healthBuff, int manaBuff, int avoidBuff, int critBuff, int movementBuff)
       {
           buff = new StatisticalBuff(healthBuff, manaBuff, avoidBuff, critBuff, movementBuff);

           Dictionary<string, int> attributes = buff.Attributes();

           attributes["strength"] = str;
           attributes["magic"] = mag;
           attributes["dexterity"] = dex;
           attributes["agility"] = agi;
           attributes["defense"] = def;
           attributes["resistance"] = res;

           this.damage = damage;
           this.accuracy = accuracy;
           this.critChance = critChance;
           this.guardChance = guardChance;
           this.durability = durability;
           this.weaponRank = weaponRank;

       }

       public int Damage() { return damage; }
       public int Accuracy() { return accuracy; }
       public int CritChance() { return critChance; }
       public int GuardChance() { return guardChance; }
       public int Durability() { return durability; }
       public int WeaponRank() { return weaponRank; }

       public StatisticalBuff Buff() { return buff; }
    }
}
