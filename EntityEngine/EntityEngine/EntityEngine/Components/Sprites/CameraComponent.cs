using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Component_Parents;

namespace EntityEngine.Components.Sprites
{
    public class CameraComponent:Component
    {
        //You must add this component to your entity if you want to use sprites. This component allows you to follow a specific entity
        //with the camera. Very useful indeed.

        Vector2 offset, followedPosition;
        public Vector2 getOffset()
        {
            return offset;
        }

        //Followed means that the camera is following this entity, following means that this entity is following another, and if
        //it's no camera the sprites are drawn to their positions without alteration
        public enum CameraState
        {
            followed, following, noCamera
        }
        public CameraState cameraState = CameraState.noCamera;

        //Call this method to set the offset to another cameras offset
        public void setFollowingCamera(Vector2 myOffset)
        {
            cameraState = CameraState.following;
            offset = myOffset;
        }

        //Call this method to calc to set this camera to following itself
        public void setFollowed(Vector2 myPosition)
        {
            cameraState = CameraState.followed;
            offset = followedPosition - myPosition;
        }

        //Pass in the position of the sprite for teh vector
        public CameraComponent(Entity myParent, Vector2 myVector):base(myParent)
        {
            this.name = "CameraComponent";
            followedPosition = new Vector2(400, 300);
            offset = followedPosition - myVector;          
        }

        //Followed means its centered on this sprite, following means its following another sprite
        public Vector2 getDrawPosition(Vector2 myVector)
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
