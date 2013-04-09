using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine
{
    class Mount
    {
        public Dictionary<string, int> attributes;

        public Mount (int hlthmod, int drb, int movemod, int resmod, int agimod, int defmod, int spdmod)
        {
            attributes = new Dictionary<string, int>();

            attributes["health modifier"] = hlthmod;
            attributes["durability"] = drb;
            attributes["move modifier"] = movemod;
            attributes["agility modifier"] = agimod;
            attributes["defense modifier"] = defmod;
            attributes["resistance modifier"] = resmod;
            attributes["speed modifier"] = spdmod;
        }
    }
}
