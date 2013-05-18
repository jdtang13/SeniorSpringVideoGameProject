using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Dialogue
{
    public class ChatBox
    {
        Actor[] actors;
        public Actor[] GetActors()
        {
            return actors;
        }

        string speaker;
        public string GetSpeaker()
        {
            return speaker;
        }
        string[] messageLines;
        public string GetMessageLine(int myIndex)
        {
            return messageLines[myIndex];
        }

        public ChatBox(string mySpeaker,Actor[] myActors,string[] myMessage)
        {
            speaker = mySpeaker;
            actors = myActors;
            messageLines = myMessage;
        }
    }
}
