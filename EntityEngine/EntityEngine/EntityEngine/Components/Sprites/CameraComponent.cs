using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Component_Parents;

namespace EntityEngine.Components.Sprites
{
    public class CameraComponent : Component
    {
        //You must add this component to your entity if you want to use sprites. This component allows you to follow a specific entity
        //with the camera. Very useful indeed.

        Vector2 offset, followedPosition,position;
        public Vector2 getOffset()
        {
            return offset;
        }

        public void PositionHasChanged(object sender, PositionArgs data)
        {
            setPosition(data.GetPosition());
            offset = position - followedPosition;
        }

        //Followed means that the camera is following this entity, following means that this entity is following another, and if
        //it's no camera the sprites are drawn to their positions without alteration
        public enum CameraState
        {
            followed, following, noCamera
        }
        public CameraState cameraState = CameraState.noCamera;

        public void SetCameraState(CameraState myState)
        {
            cameraState = myState;
        }

        // TODO: added by jon. ask oliver to debug.
        public void setPosition(Vector2 pos)
        {
            position = pos;
        }

        //Pass in the position of the sprite for teh vector
        public CameraComponent(Vector2 myVector)
        {
            this.name = "CameraComponent";
            followedPosition = new Vector2(640, 340);
            
            position = myVector;
        }

        //Followed means its centered on this sprite, following means its following another sprite
        public Vector2 GetDrawPosition(Vector2 myVector)
        {
            if (cameraState == CameraState.following)
            {
                return myVector + offset;
            }
            else if (cameraState == CameraState.followed)
            {
                return followedPosition;
            }
            else
            {
                return myVector;
            }

        }
    }
}
