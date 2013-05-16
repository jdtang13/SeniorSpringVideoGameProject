using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EntityEngine.Dialogue
{
    //public enum ChatSlot
    //{
    //    Left, LeftCenter, RightCenter, Right
    //}
    public enum Emotion
    {
        Smile, Scowl, Frown, Laugh, Neutral, None
    }

    public class Actor
    {
        //ChatSlot slot;
        int slot;
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

        Vector2[] actorPositions = new Vector2[4]; Vector2 position;

        public Actor(PortraitPackage myPackage, Emotion myEmotion, int mySlot,Vector2[] myActorPositions, Boolean myIsSpeaker)
        {
            actorPositions = myActorPositions;

            portraitPackage = myPackage;
            emotion = myEmotion;
            isSpeaker = myIsSpeaker;

            slot = mySlot;
            position = actorPositions[slot];

            //if (mySlot == 0)
            //{
            //    slot = ChatSlot.Left;
            //}
            //else if(mySlot == 1)
            //{
            //    slot = ChatSlot.LeftCenter;
            //}
            //else if (mySlot == 2)
            //{
            //    slot = ChatSlot.RightCenter;
            //}
            //else
            //{
            //    slot = ChatSlot.Right;
            //}
            
        }

        public void Draw(SpriteBatch myBatch)
        {
            
            switch (slot)
            {
                case 0:
                    if (isSpeaker)
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 0).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion,0), position-offset, Color.White);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 0).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 0), position - offset, Color.White);
                    }
                    break;
                case 1:
                    if (isSpeaker)
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 0).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 0), position - offset, Color.White);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 0).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 0), position - offset, Color.White);
                    }
                    break;
                case 2:
                    if (isSpeaker)
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 1).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 1), position - offset, Color.White);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 1).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 1), position - offset, Color.White);
                    }
                    break;
                case 3:
                    if (isSpeaker)
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 1).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 1), position - offset, Color.White);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(portraitPackage.GetTexture(emotion, 1).Width / 2, portraitPackage.GetTexture(emotion, 0).Height / 2);
                        myBatch.Draw(portraitPackage.GetTexture(emotion, 1), position - offset, Color.White);
                    }
                    break;
                default:
                    break;
            }
            //Draw this shit
        }
    }
}
