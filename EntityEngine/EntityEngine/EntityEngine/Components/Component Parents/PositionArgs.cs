using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityEngine.Components.Component_Parents
{
    public class PositionArgs : System.EventArgs
    {
        Vector2 position;

        public PositionArgs(Vector2 v)
        {
            position = v;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    } 
}
