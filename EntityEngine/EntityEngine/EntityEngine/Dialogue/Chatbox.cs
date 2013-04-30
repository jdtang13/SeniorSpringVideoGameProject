using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine.Dialogue
{
    public enum ChatEvent
    {
        Beginning, Victory, Defeat
    }

    public enum ChatboxStatus
    {
        Writing, WaitingInput, WaitingNext, Idle
    }

    public static class Chatbox
    {
        static SpriteFont font;
        static Texture2D backdrop;
        static Dictionary<string, PortraitPackage> portraitDictionary = new Dictionary<string, PortraitPackage>();

        static Boolean active;

        static ChatboxStatus status = ChatboxStatus.Idle;
        public static ChatboxStatus GetStatus()
        {
            return status;
        }

        static ChatEvent currentEvent;
        public static ChatEvent GetEvent()
        {
            return currentEvent;
        }

        static List<string> uninterpretedMessages = new List<string>();

        static string speakerName;
        static string currentWritten = ""; static int currentCharacter = 0;
        static string message; static char[] messageCharacters;

        static int currentBox = 0; static int numberOfBoxes;

        public static float slowTypingSpeed = 200f; public static float fastTypingSpeed = 15f;
        
        //public static int dialoguePosition = 0;
        //public static int dialogueChoicePosition = 0;
        //public static string displayedDialogueMessage = "";

        //public static bool messageBegin = false;
        //public static int dialogueLinePosition = 0;
        //public static int dialogueWordPosition = 0;
        //public static int dialogueCharacterPosition;
        //public static string firstDialogueWord = "";
        //public static List<string> currentDialogueMessage = new List<string>();
        
        static Portrait[] portraitArray = new Portrait[4];

        public static void Initialize(Dictionary<string, PortraitPackage> myDictionary, SpriteFont myFont, Texture2D myBackdrop)
        {
            portraitDictionary = myDictionary;
            font = myFont;
            backdrop = myBackdrop;
        }

        //The function is given new messages from the Game1 class as a new event is raised
        //It also reactivates the chatbox
        public static void SetNewInfo(List<String> myList)
        {
            uninterpretedMessages = myList;
            currentBox = 0; numberOfBoxes = myList.Count;
            status = ChatboxStatus.WaitingNext;
            active = true;
            portraitArray = new Portrait[4];
        }

        public static void Update(GameTime myTime)
        {
            if (active)
            {
                if (status == ChatboxStatus.WaitingNext)
                {
                    //Filling the chatboxes
                    string line = uninterpretedMessages[currentBox];
                    string[] chatboxParameters = line.Split(new string[] { " ; " }, StringSplitOptions.None);

                    string[] characters = chatboxParameters[0].Split(new string[] { " , " }, StringSplitOptions.None);

                    for (int p = 0; p < 4; p++)
                    {
                        if (Convert.ToInt32(chatboxParameters[1]) == p)
                        {
                            if (characters[p] != "null")
                            {
                                string[] characterInfo = characters[p].Split(' ');
                                portraitArray[p] = new Portrait(portraitDictionary[characterInfo[0]],
                                    (Emotion)Enum.Parse(typeof(Emotion), characterInfo[1]), p, true);
                                speakerName = characterInfo[0];
                            }
                        }
                        else
                        {
                            if (characters[p] != "null")
                            {
                                string[] characterInfo = characters[p].Split(' ');
                                portraitArray[p] = new Portrait(portraitDictionary[characterInfo[0]],
                                    (Emotion)Enum.Parse(typeof(Emotion), characterInfo[1]), p, false);
                            }
                        }
                    }
                    int speaker = Convert.ToInt32(chatboxParameters[1]);
                    message = chatboxParameters[2].Split(new string[] { " : " }, StringSplitOptions.None)[1];
                    messageCharacters = message.ToCharArray();

                    currentCharacter = 0;

                    status = ChatboxStatus.Writing;
                }


                else if (status == ChatboxStatus.Writing)
                {
                    if (myTime.TotalGameTime.TotalMilliseconds - State.lastTimeDialogueChecked > slowTypingSpeed)
                    {
                        if (currentCharacter < messageCharacters.Length)
                        {
                            currentWritten += messageCharacters[currentCharacter];
                            currentCharacter++;
                        }
                        else
                        {
                            //We have reached the end of the message, time to click next
                            status = ChatboxStatus.WaitingInput;
                        }

                        //if (messageBegin)
                        //{
                        //    if (currentWord == "]")
                        //    {
                        //        State.messageBegin = false;
                        //        State.displayedDialogueMessage += "\n"; // newlines for new messages

                        //        State.dialogueLinePosition++;
                        //        State.dialogueWordPosition = 0;
                        //    }
                        //    else
                        //    {
                        //        State.displayedDialogueMessage += currentChar;
                        //        //  add chars blipping onto the screen
                        //    }

                        //}
                        //else
                        //{
                        //    State.messageBegin = (currentWord == "[");
                        //}

                        //if (State.dialogueCharacterPosition == currentWord.Count())
                        //{
                        //    if (State.dialogueWordPosition != 0)
                        //    {
                        //        State.displayedDialogueMessage += " ";
                        //    }
                        //    State.dialogueCharacterPosition = 0;
                        //    State.dialogueWordPosition++;
                        //}

                        State.lastTimeDialogueChecked = (int)myTime.TotalGameTime.TotalMilliseconds;
                    }
                }
                else
                {
                    //Wait for the input
                    //Blink cursor?
                }
                
                

                
            }
        }

        public static void RushTyping()
        {
            //Type whole message quickly
        }

        //Called after user clicks to say display next
        public static void Advance()
        {
            //IF we aren't at the end of the chatboxes
            if (currentBox >= numberOfBoxes)
            {
                currentBox++;
                Chatbox.status = ChatboxStatus.WaitingNext;
            }
            else
            {
                status = ChatboxStatus.Idle;
                active = false;
            }
        }

        public static void Draw(SpriteBatch myBatch)
        {
            if (active)
            {
                //for (int p = 0; p < 4; p++)
                //{
                //    if (portraitArray[p] != null)
                //    {
                //        portraitArray[p].Draw(myBatch);
                //    }
                //}
                
                //myBatch.Draw(backdrop, Vector2.Zero, Color.White);

                myBatch.DrawString(font, speakerName, new Vector2(300,300), Color.White);
                myBatch.DrawString(font, currentWritten, new Vector2(300, 350), Color.White);

                //Draw name
                //Draw currentText
                //Right now we only have a long string that doesnt get seperated
            }
        }
    }
}
