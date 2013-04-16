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

        Vector2 followingOffset, followedOffset, centerScreen, position;

        public Vector2 GetFollowedOffset()
        {
            return followedOffset;
        }
        public void SetFollowingOffset(Vector2 myVector)
        {
            followingOffset = myVector;
            cameraState = CameraState.following;
        }

        public void PositionHasChanged(object sender, PositionArgs data)
        {
            position = data.GetPosition();
            followedOffset = centerScreen - position;
            
            //Send pulse to tell it refresh
            if (cameraState == CameraState.followed)
            {
                EntityManager.FollowEntity(this._parent);
            }
        }

        public enum CameraState
        {
            followed, following, noCamera
        }
        public CameraState cameraState = CameraState.noCamera;

        public void SetCameraState(CameraState myState)
        {
            cameraState = myState;
        }


        //Pass in the position of the sprite for teh vector
        public CameraComponent(Vector2 myVector)
        {
            this.name = "CameraComponent";
            centerScreen = new Vector2(640, 340);
            
            position = myVector;
        }

        //Followed means its centered on this sprite, following means its following another sprite
        public Vector2 GetDrawPosition(Vector2 myVector)
        {
            if (cameraState == CameraState.following)
            {
                return myVector + followedOffset;
            }
            else if (cameraState == CameraState.followed)
            {
                return centerScreen;
            }
            else
            {
                return myVector;
            }

        }
    }
}
