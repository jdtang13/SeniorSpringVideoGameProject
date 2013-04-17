using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Component_Parents;

namespace EntityEngine.Components.Sprites
{
<<<<<<< HEAD
    public class CameraComponent:Component
=======
    public class CameraComponent : Component
>>>>>>> origin/Oliver
    {
        //You must add this component to your entity if you want to use sprites. This component allows you to follow a specific entity
        //with the camera. Very useful indeed.

        Vector2 offset, centerScreen, position;

        public Vector2 GetOffset()
        {
            offset = centerScreen - position;
            return offset;
        }
        public void SetOffset(Vector2 myVector)
        {
            offset = myVector;
        }

<<<<<<< HEAD
        //Followed means that the camera is following this entity, following means that this entity is following another, and if
        //it's no camera the sprites are drawn to their positions without alteration
=======
        public void PositionHasChanged(object sender, PositionArgs data)
        {
            position = data.GetPosition();

            //Send pulse to tell it refresh
            //if (cameraState == CameraState.followed)
            //{
            //    EntityManager.FollowEntity(this._parent);
            //    offset = centerScreen - position;
            //}
        }

>>>>>>> origin/Oliver
        public enum CameraState
        {
            followed, following, noCamera
        }
        public CameraState cameraState = CameraState.noCamera;

        public void SetCameraState(CameraState myState)
        {
            cameraState = myState;
            if (cameraState == CameraState.followed)
            {
                offset = centerScreen - position;
            }
        }

        // TODO: added by jon. ask oliver to debug.
        public void setPosition(Vector2 pos)
        {
            offset = followedPosition - pos;
        }

        //Pass in the position of the sprite for teh vector
<<<<<<< HEAD
        public CameraComponent( Vector2 myVector)
        {
            this.name = "CameraComponent";
            followedPosition = new Vector2(400, 300);
            offset = followedPosition - myVector;          
=======
        public CameraComponent(Vector2 myVector)
        {
            this.name = "CameraComponent";
            centerScreen = new Vector2(640, 340);
            
            position = myVector;
            offset = centerScreen - position;
>>>>>>> origin/Oliver
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
                return centerScreen;
            }
            else
            {
                return myVector;
            }

        }
    }
}
