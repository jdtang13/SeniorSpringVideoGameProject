using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Dialogue
{
    public enum ChatSlot
    {
        Left, LeftCenter, RightCenter, Right
    }
    public enum Emotion
    {
        Smile, Scowl, Frown, Laugh, Neutral, None
    }

    public class Actor
    {
        ChatSlot slot;
        Boolean isSpeaker;

        PortraitPackage portraitPackage;
        public void SetPortraitPackage(PortraitPackage myPackage)
        {
            portraitPackage = myPackage;
        }
        public PortraitPackage GetPortraitPackage()
        {
            return portraitPackage;
        }

        Emotion emotion;
        public void SetEmotion(Emotion myEmotion)
        {
            emotion = myEmotion ;
        }
        public Emotion GetEmotion()
        {
            return emotion;
        }

        public Actor(PortraitPackage myPackage, Emotion myEmotion, int mySlot, Boolean myIsSpeaker)
        {
            portraitPackage = myPackage;
            emotion = myEmotion;
            isSpeaker = myIsSpeaker;
            if (mySlot == 0)
            {
                slot = ChatSlot.Left;
            }
            else if(mySlot == 1)
            {
                slot = ChatSlot.LeftCenter;
            }
            else if (mySlot == 2)
            {
                slot = ChatSlot.RightCenter;
            }
            else
            {
                slot = ChatSlot.Right;
            }
            
        }

        public void Draw(SpriteBatch myBatch)
        {
            switch (slot)
            {
                case ChatSlot.Left:
                    if (isSpeaker)
                    {

                    }
                    break;
                case ChatSlot.LeftCenter:
                    if (isSpeaker)
                    {

                    }
                    break;
                case ChatSlot.RightCenter:
                    if (isSpeaker)
                    {

                    }
                    break;
                case ChatSlot.Right:
                    if (isSpeaker)
                    {

                    }
                    break;
                default:
                    break;
            }
            //Draw this shit
        }
    }
}
