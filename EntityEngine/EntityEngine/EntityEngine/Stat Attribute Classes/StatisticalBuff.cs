using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Stat_Attribute_Classes
{
    //  a buff can come from a bard's song, a potion, a piece of armor, 
    //  support synergy, etc.
    class StatisticalBuff : StatisticalEntity
    {
        int healthBuff;
        int manaBuff;
        int avoidBuff;
        int critBuff;
        int movementBuff;

        public StatisticalBuff(int healthBuff, int manaBuff, int avoidBuff, int critBuff, int movementBuff)
        {
            this.healthBuff = healthBuff;
            this.manaBuff = manaBuff;
            this.avoidBuff = avoidBuff;
            this.critBuff = critBuff;
            this.movementBuff = movementBuff;
        }

        public int HealthBuff() { return healthBuff; }
        public int ManaBuff() { return manaBuff; }
        public int AvoidBuff() { return avoidBuff; }
        public int CritBuff() { return critBuff; }
    }
}
