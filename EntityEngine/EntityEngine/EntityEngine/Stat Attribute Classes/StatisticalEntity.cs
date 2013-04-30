using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Stat_Attribute_Classes
{
    public class StatisticalEntity
    {
        protected Dictionary<string, int> attributes = new Dictionary<string, int>();
        protected Dictionary<string, float> growths = new Dictionary<string, float>();
        protected Dictionary<string, int> caps = new Dictionary<string, int>();

        public StatisticalEntity(int str, int mag, int dex, int agi, int def, int res,
                    float strGrowth, float magGrowth, float dexGrowth, float agiGrowth, float defGrowth, float resGrowth,
                    int strCap, int magCap, int dexCap, int agiCap, int defCap, int resCap)
        {
            attributes["strength"] = str;
            attributes["magic"] = mag;
            attributes["dexterity"] = dex;
            attributes["agility"] = agi;
            attributes["defense"] = def;
            attributes["resistance"] = res;

            growths["strength"] = strGrowth;
            growths["magic"] = magGrowth;
            growths["dexterity"] = dexGrowth;
            growths["agility"] = agiGrowth;
            growths["defense"] = defGrowth;
            growths["resistance"] = resGrowth;

            caps["strength"] = strCap;
            caps["magic"] = magCap;
            caps["dexterity"] = dexCap;
            caps["agility"] = agiCap;
            caps["defense"] = defCap;
            caps["resistance"] = resCap;
        }

        public StatisticalEntity()
        {
        }

        public void SetAttributes(Dictionary<string, int> attributes)
        {
            this.attributes = attributes;
        }

        public void SetGrowths(Dictionary<string, float> growths)
        {
            this.growths = growths;
        }

        public void SetCaps(Dictionary<string, int> caps)
        {
            this.caps = caps;
        }

        public Dictionary<string, int> Attributes()
        {
            return attributes;
        }

        public Dictionary<string, float> Growths()
        {
            return growths;
        }

        public Dictionary<string, int> Caps()
        {
            return caps;
        }
    }
}
