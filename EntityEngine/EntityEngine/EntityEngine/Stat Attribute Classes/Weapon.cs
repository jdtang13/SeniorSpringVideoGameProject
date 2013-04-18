using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine
{
    class Weapon
    {
       public Dictionary<string, int> attributes;

       public Weapon(int dmg, int drb, int htc, int critc, int critd, int wprofr,
           int dexmod, int resmod, int spdmod, int agimod)
       {
           attributes = new Dictionary<string, int>();

           attributes["damage"] = dmg;
           attributes["durability"] = drb;
           attributes["hit chance"] = htc;
           attributes["critical strike chance"] = critc;
           attributes["critical strike damage"] = critd;
           attributes["weapon proffeciency requirement"] = wprofr;
           attributes["dexterity modifier"] = dexmod;
           attributes["agility modifier"] = agimod;
           attributes["resistance modifier"] = resmod;
           attributes["speed modifier"] = spdmod;
       }
    }
}
