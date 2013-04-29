using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;

namespace EntityEngine.Components.World_Map
{
    public class ClickableComponent : Component
    {
        Rectangle collisionRectangle;
        public bool isColliding(Vector2 myVector)
        {
            return (collisionRectangle.Contains((int)myVector.X,(int)myVector.Y));
        }

        // myCenterPosition = the position that you WANT the center to be at
        public ClickableComponent(Vector2 myCenterPosition, int mySpriteWidth, int mySpriteHeight)
        {
            this.name = "ClickableComponent";
            collisionRectangle = new Rectangle((int)myCenterPosition.X - mySpriteWidth / 2, (int)myCenterPosition.Y - mySpriteHeight / 2,
                                                mySpriteWidth, mySpriteHeight);
        }
    }
}
