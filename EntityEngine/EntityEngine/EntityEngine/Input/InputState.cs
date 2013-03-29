using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using EntityEngine.Input;


namespace EntityEngine.Input
{
    public static class InputState
    {
        //A class that does not need to be instatiated to work. I'ts update function is called in the entity manager for ease of use.
        //Keeps a picture of what your keyboard looks like so you can ask it questions later.

       
        public static MouseState newMouseState = new MouseState();
        public static MouseState oldMouseState = new MouseState();

        public static KeyboardState newKeyboardState = new KeyboardState();
        public static KeyboardState oldKeyboardState = new KeyboardState();

        public static Vector2 getMousePosition()
        {
            return new Vector2(newMouseState.X, newMouseState.Y);
        }

        public static void Update()
        {
            oldKeyboardState = newKeyboardState;
            newKeyboardState = Keyboard.GetState();

            oldMouseState = newMouseState;
            newMouseState = Mouse.GetState();
        }

        public static bool IsMousePressed(MouseButton myButton)
        {
            if (myButton == MouseButton.left)
            {
                if (newMouseState.LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (myButton == MouseButton.right)
            {
                if (newMouseState.RightButton == ButtonState.Pressed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (newMouseState.MiddleButton == ButtonState.Pressed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsNewMousePressed(MouseButton myButton)
        {
            if (myButton == MouseButton.left)
            {
                if (newMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (oldMouseState.LeftButton == ButtonState.Released)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            if (myButton == MouseButton.right)
            {
                if (newMouseState.RightButton == ButtonState.Pressed)
                {
                    if (oldMouseState.RightButton == ButtonState.Released)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (newMouseState.MiddleButton == ButtonState.Pressed)
                {
                    if (oldMouseState.MiddleButton == ButtonState.Released)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            return newKeyboardState.IsKeyDown(key);
        }

        public static bool IsNewKeyPress(Keys key)
        {
            return (newKeyboardState.IsKeyDown(key) &&
                    oldKeyboardState.IsKeyUp(key));
        }
    }
}
