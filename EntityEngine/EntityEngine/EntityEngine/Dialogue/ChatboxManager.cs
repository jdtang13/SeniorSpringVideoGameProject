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
    //public enum ChatEvent
    //{
    //    Beginning, Victory, Defeat
    //}

    public enum ChatboxStatus
    {
        Writing, WaitingInput, WaitingNext, Idle, Finished
    }

    public static class ChatboxManager
    {
        //1280,680
        static Vector2 screenSize = new Vector2(1280, 680);
        static Vector2 screenCenter = screenSize / 2;
        static Vector2 backdropPosition;
        static Vector2 offset = new Vector2(0, 0);

        static Vector2 bounds;
        static Vector2 speakerNameStartingPosition;
        static Vector2 messageStartingPosition;
        static int maxCharactersPerLine;
        static int maxLines;
        static int maxCharactersPerBox;

        static SpriteFont font;
        static Texture2D backdrop;

        static Boolean active;
        static ChatboxStatus status = ChatboxStatus.Idle;
        public static ChatboxStatus GetStatus()
        {
            return status;
        }

        static List<string> uninterpretedMessages = new List<string>();

        static string speakerName;
        static int currentLine = 0;
        static string[] currentWritten; static int currentCharacter = 0;
        static string[] messageLines; static char[] messageCharacters;

        static Dictionary<string, PortraitPackage> portraitDictionary;
        static Vector2[] actorPositions = new Vector2[4];
        static Actor[] actorArray = new Actor[4];

        static List<ChatBox> chatboxes = new List<ChatBox>();
        static int currentChatboxIndex = 0; static ChatBox currentChatbox;

        static string eventName;

        static float slowTypingSpeed = 50f; public static float fastTypingSpeed = .001f;
        static float currentTypingSpeed = slowTypingSpeed;
        static float timeSinceLastCharacter = 0f;

        public static void Initialize(Dictionary<string, PortraitPackage> myDictionary, SpriteFont myFont, Texture2D myBackdrop, Vector4 myMargins)
        {
            portraitDictionary = myDictionary;
            font = myFont;
            backdrop = myBackdrop;

            //Setting margins and positions of the chatbox
            backdropPosition = new Vector2(screenCenter.X - backdrop.Width / 2, screenCenter.Y - backdrop.Height / 2);
            backdropPosition += offset;

            float topMargin = myMargins.W; float rightMargin = myMargins.X;
            float bottomMargin = myMargins.Y; float leftMargin = myMargins.Z;

            speakerNameStartingPosition = new Vector2(backdropPosition.X + leftMargin / 2f, backdropPosition.Y);

            messageStartingPosition = new Vector2(backdropPosition.X + leftMargin, backdropPosition.Y + topMargin);

            maxCharactersPerLine = Convert.ToInt32((backdrop.Width - leftMargin - rightMargin) / font.MeasureString("W").X);

            maxLines = Convert.ToInt32((backdrop.Height - topMargin - bottomMargin) / font.LineSpacing);
        }

        //The function is given new messages from the Game1 class as a new event is raised
        //It also reactivates the chatbox
        //This is the event's text that was requested; no other events are brought in
        public static void SetNewInfo(List<String> myList)
        {
            uninterpretedMessages = myList;

            actorArray = new Actor[4];
            speakerName = "";
            messageLines = new string[maxLines];

            active = true;
            status = ChatboxStatus.WaitingNext;
        }

        public static void Update(GameTime myTime)
        {
            if (active)
            {
                if (status == ChatboxStatus.WaitingNext)
                {
                    chatboxes = new List<ChatBox>();

                    for (int line = 0; line < uninterpretedMessages.Count; line++)
                    {
                        //We found a new chatbox
                        if (uninterpretedMessages[line].Contains(" ; "))
                        {

                            string[] chatboxParameters = uninterpretedMessages[line].Split(new string[] { " ; " }, StringSplitOptions.None);


                            Actor[] tempActorArray = new Actor[4];

                            string tempSpeakerName = "";

                            //Creating the actors array for the chatbox
                            for (int p = 0; p < 4; p++)
                            {
                                string[] characterInfo = chatboxParameters[p].Split(new string[] { " , " }, StringSplitOptions.None);
                                if (characterInfo[0] != "None")
                                {
                                    if (ConvertDirectionToNumber(chatboxParameters[4]) == p)
                                    {
                                        tempActorArray[p] = new Actor(portraitDictionary[characterInfo[0]],
                                            (Emotion)Enum.Parse(typeof(Emotion), characterInfo[1]), p, true);

                                        tempSpeakerName = characterInfo[0];
                                    }
                                    else
                                    {
                                        tempActorArray[p] = new Actor(portraitDictionary[characterInfo[0]],
                                            (Emotion)Enum.Parse(typeof(Emotion), characterInfo[1]), p, false);
                                    }
                                }
                            }

                            string tempMessage = "";

                            //We are fetching the message of the chatbox here
                            for (int messageIndex = line + 1; messageIndex < uninterpretedMessages.Count; messageIndex++)
                            {
                                if (messageIndex < uninterpretedMessages.Count)
                                {
                                    if (uninterpretedMessages[messageIndex].Contains(" ; "))
                                    {
                                        //If we reach the next chatbox we stop what we are doing
                                        break;
                                    }
                                    //TODO: FIX THE ERROR
                                    tempMessage += uninterpretedMessages[messageIndex] + " ";
                                }
                            }

                            int charactersUsedOnCurrentLine = 0;
                            string[] tempMessageWords = tempMessage.Split(' ');

                            string[] chatboxSpecificMessageLines = new string[maxLines];
                            int currentLine = 0;

                            //Go through everyword and put it on a line
                            for (int w = 0; w < tempMessageWords.Length; w++)
                            {
                                if (charactersUsedOnCurrentLine + tempMessageWords[w].ToCharArray().Length <= maxCharactersPerLine)
                                {
                                    //If we can fit the word on the line put it in
                                    chatboxSpecificMessageLines[currentLine] += tempMessageWords[w] + " ";

                                    charactersUsedOnCurrentLine += tempMessageWords[w].ToCharArray().Length;
                                    charactersUsedOnCurrentLine++;//The space
                                }
                                else
                                {
                                    currentLine++;
                                    charactersUsedOnCurrentLine = 0;

                                    if (currentLine < maxLines)
                                    {
                                        //The word is placed on the next lien
                                        chatboxSpecificMessageLines[currentLine] += tempMessageWords[w] + " ";
                                        charactersUsedOnCurrentLine += tempMessageWords[w].ToCharArray().Length;
                                        charactersUsedOnCurrentLine++;
                                    }
                                    else
                                    {
                                        //Create a new chatbox, we're at the extent of this one
                                        ChatBox box = new ChatBox(tempSpeakerName, tempActorArray, chatboxSpecificMessageLines);
                                        chatboxes.Add(box);
                                        chatboxSpecificMessageLines = new string[maxLines];
                                        currentLine = 0;
                                    }
                                }
                            }
                            if (charactersUsedOnCurrentLine != 0)
                            {
                                ChatBox box = new ChatBox(tempSpeakerName, tempActorArray, chatboxSpecificMessageLines);
                                chatboxes.Add(box);
                            }

                        }
                    }

                    currentCharacter = 0;
                    currentWritten = new string[maxLines];
                    currentChatbox = chatboxes[currentChatboxIndex];
                    status = ChatboxStatus.Writing;
                }


                else if (status == ChatboxStatus.Writing)
                {
                    messageCharacters = currentChatbox.GetMessageLine(currentLine).ToCharArray();
                    speakerName = currentChatbox.GetSpeaker();

                    timeSinceLastCharacter += (float)myTime.ElapsedGameTime.TotalMilliseconds;

                    if (timeSinceLastCharacter > currentTypingSpeed)
                    {
                        timeSinceLastCharacter -= currentTypingSpeed;
                        if (currentCharacter < messageCharacters.Length)
                        {
                            int p = (int)(State.lastTimeDialogueChecked / currentTypingSpeed);
                            currentWritten[currentLine] += messageCharacters[currentCharacter];
                            currentCharacter++;
                        }
                        else
                        {
                            currentLine++;
                            currentCharacter = 0;

                            if (currentLine < maxLines)
                            {
                                //We're just moving to the next line
                                messageCharacters = currentChatbox.GetMessageLine(currentLine).ToCharArray();
                            }
                            else
                            {
                                //We have reached the end of the lines, time to click next
                                currentLine = 0;
                                status = ChatboxStatus.WaitingInput;
                            }
                        }
                    }
                }
                else if (status == ChatboxStatus.WaitingInput)
                {
                    //Wait for the input
                    //Blink cursor?
                }
            }
        }

        public static int ConvertDirectionToNumber(string myString)
        {
            if (myString == "Left")
            {
                return 0;
            }
            else if (myString == "Left Center")
            {
                return 1;
            }
            else if (myString == "Right Center")
            {
                return 2;
            }
            else if (myString == "Right")
            {
                return 3;
            }
            else
            {
                //If its none return arbitrary wrong number
                return 10101010;
            }

        }

        public static void RushTyping()
        {
            //Type whole tempMessage quickly
            currentTypingSpeed = fastTypingSpeed;
        }

        //Called after user clicks to say display next
        public static void Advance()
        {
            //IF we aren't at the end of the chatboxes
            if (currentChatboxIndex < chatboxes.Count)
            {
                currentChatboxIndex++;
                currentChatbox = chatboxes[currentChatboxIndex];
                ChatboxManager.status = ChatboxStatus.WaitingNext;
                currentTypingSpeed = slowTypingSpeed;
            }
            else
            {
                //THAT'S THE WHOLE DIALOGUE SESSION
                //TODO: Somehow quit since we are doen with dialogue
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
                //    if (actorArray[p] != null)
                //    {
                //        actorArray[p].Draw(myBatch);
                //    }
                //}

                myBatch.Draw(backdrop, backdropPosition, Color.White);

                myBatch.DrawString(font, speakerName, speakerNameStartingPosition, Color.Green);
                if (currentWritten != null)
                {
                    for (int line = 0; line < currentWritten.Length; line++)
                    {
                        if (currentWritten[line] != null)
                        {

                            myBatch.DrawString(font,
                                                currentWritten[line],
                                                new Vector2(messageStartingPosition.X,
                                                messageStartingPosition.Y + (line * font.MeasureString(" ").Y)),
                                                Color.Black
                                              );
                        }
                    }
                }
            }
        }
    }
}
