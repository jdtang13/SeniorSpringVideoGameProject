using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Dialogue
{
    public class Chatbox
    {
        int speakerID;
        string speakerName;
        string message;

        public Chatbox(int myID, string myName, string myMessage)
        {
            speakerID = myID;
            speakerName = myName;
            message = myMessage;
        }
        //public void AddPerson(PortraitPackage myPackage, ChatSlot mySlot, bool myActive)
        //{

        //}
    }
}
