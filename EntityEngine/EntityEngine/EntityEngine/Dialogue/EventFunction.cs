using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Dialogue
{
    public class EventFunction
    {
        public string eventName = "";

        public EventFunction(string myEvent)
        {
            eventName = myEvent;
        }

        public void Run()
        {
            if (eventName == "E1")
            {

            }
            else if (eventName == "KirateKidWasALie")
            {

            }
        }
    }
}
