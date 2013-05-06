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

        int duration; //  a buff might be permanent (such as a buff on a weapon) or temporary
                      //  (such as from a spell or song or potion). duration is measured in "turns"

        public StatisticalBuff(int healthBuff, int manaBuff, int avoidBuff, int critBuff, int movementBuff)
        {
            this.healthBuff = healthBuff;
            this.manaBuff = manaBuff;
            this.avoidBuff = avoidBuff;
            this.critBuff = critBuff;
            this.movementBuff = movementBuff;

            this.duration = 0; //  duration of zero = permanent buff
        }

        public int Duration() { return duration; }
        public void SetDuration(int duration) { this.duration = duration; }

        public int HealthBuff() { return healthBuff; }
        public int ManaBuff() { return manaBuff; }
        public int AvoidBuff() { return avoidBuff; }
        public int CritBuff() { return critBuff; }
    }
}
