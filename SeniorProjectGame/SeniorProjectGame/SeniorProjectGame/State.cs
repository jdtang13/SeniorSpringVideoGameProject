using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorProjectGame
{
    public class State
    {
        public enum ScreenState
        {
            BATTLING, SKIRMISH, BATTLE_FORECAST, WORLD_MAP, SHOP, SETTINGS_MENU, MAIN_PAGE, DIALOGUE, BATTLE_RESOLUTION
            //SELECTING_UNIT_ON_SKIRMISH_MAP, SELECTING_OPTIONS_FOR_SKIRMISH_UNITS
        }

        public enum SelectionState
        {
            SelectingUnit, SelectingOptionsForSkirmishUnits, NoSelection
        }

        //  use this for selecting battles and stuff
        public enum MenuState
        {

        }

        public static int menuPosition = 0;

        public static ScreenState screenState;// default = ScreenState.MAIN_PAGE;
        public static SelectionState selectionState;

        //public Node currentNode;
        public static int dialoguePosition = 0;
        public static int dialogueChoicePosition = 0;
        public static string displayedDialogueMessage = "";

        public static bool messageBegin = false;
        public static int dialogueLinePosition = 0;
        public static int dialogueWordPosition = 0;
        public static int dialogueCharacterPosition;
        public static string firstDialogueWord = "";
        public static List<string> currentDialogueMessage = new List<string>();

        public static int lastTimeDialogueChecked; // TODO

        //public static List<Unit> units;
    }
}
