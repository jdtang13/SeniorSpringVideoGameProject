using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using EntityEngine.Input;

namespace EntityEngine.Input
{
    public class InputAction
    {
        private readonly Keys[] keys;
        private readonly MouseButton button;

        private readonly bool newPressOnly;

        private delegate bool KeyPress(Keys myKey);
        private delegate bool MousePress(MouseButton myButton);

        public enum InputType
        {
            keyboard, mouse
        }
        InputType inputType;

        //Constructor for keyboard events
        public InputAction(Keys[] myKeys, bool myNewPressOnly)
        {
            keys = myKeys;
            newPressOnly = myNewPressOnly;
            inputType = InputType.keyboard;
        }

        //Constructor for mouse actions
        public InputAction(MouseButton myButton, bool myNewPressOnly)
        {
            button = myButton;
            newPressOnly = myNewPressOnly;
            inputType = InputType.mouse;
        }

        //Delegates used to determine which method to use.
        public bool Evaluate()
        {
            KeyPress keyDelegate = null;
            MousePress mouseDelegate = null;

            if (inputType == InputType.keyboard)
            {
                if (newPressOnly)
                {
                    keyDelegate = InputState.IsNewKeyPress;
                }
                else
                {
                    keyDelegate = InputState.IsKeyPressed;
                }
            }
            else
            {
                if (newPressOnly)
                {
                    mouseDelegate = InputState.IsNewMousePressed;
                }
                else
                {
                    mouseDelegate = InputState.IsMousePressed;
                }
            
            }

            //Here is where we actually run the methods
            if (inputType == InputType.keyboard)
            {
                foreach (Keys key in keys)
                {
                    if (keyDelegate(key))
                        return true;
                }
            }
            else
            {
                if (mouseDelegate(button))
                    return true;
            }

            return false;
        }
    }
}
