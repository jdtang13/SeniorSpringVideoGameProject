using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Sprites;
using EntityEngine.Components.Component_Parents;
using EntityEngine.Input;


namespace EntityEngine
{
    public static class EntityManager
    {
        //Where all the entities get moved about. If you want an entity to be handled, add it to the entity manager. The update
        //and draw functions automatically get called.

        //There is only one entity manager, as it is a static class. Meaning you don't need to create an instance of it or use a 
        //constructor. If you want to add an entity:
        //      EntityManager.AddEntity(SpaceShip);


        public static List<Entity> masterList = new List<Entity>();

        
        public static void AddEntity(Entity myEntity)
        {
            masterList.Add(myEntity);
        }
        public static void ClearEntities()
        {
            masterList.Clear();
            //currentList.Clear();
        }

        //Max of twenty different layers that an entity can exist on. Obviously you can change this number.
        static int LAYER_LIMIT = 20;

        public static void Update(GameTime myTime)
        {
            InputState.Update();

            for (int p = 0; p < masterList.Count; p++)
            {
                if (masterList[p].GetAssociatedState() == State.screenState)
                {
                    masterList[p].Update(myTime);
                }
            }
        }

        public static void FollowEntity(Entity myEntity)
        {
            //Grab the followed entity's camera so we can edit it later, we assume it has a camera object
            CameraComponent followedCamera = myEntity.GetComponent("CameraComponent") as CameraComponent;

            //Cycle through every drawable component the followed entity has
            for (int o = 0; o < myEntity.drawableComponentList.Count; o++)
            {
                DrawableComponent draw = myEntity.drawableComponentList[o] as DrawableComponent;

                //Check if its the draw component is the main drawable component
                if (draw.isMainSprite)
                {
                    //Set the camerea to followed cameras offset using the screenPosition of the main sprite
                    followedCamera.setFollowed(draw.position);
                }
            }
            for (int p = 0; p < masterList.Count; p++)
            {
                //Grab every entities cam object,if it has one, and apply its transformation to all
                CameraComponent cam = masterList[p].GetComponent("CameraComponent") as CameraComponent;
                if (cam != null)
                {
                    //Set the camera to following by the followed entity's offset
                    cam.setFollowingCamera(followedCamera.getOffset());
                }
            }
        }


        public static void Draw(SpriteBatch myBatch)
        {
            //Cycle through the layers of all the entities, 0 being the most background
            for (int q = 0; q < LAYER_LIMIT; q++)
            {
                for (int p = 0; p < masterList.Count; p++)
                {
                    if (masterList[p].layer == q)
                    {
                        if (masterList[p].GetAssociatedState() == State.screenState)
                        {
                            masterList[p].Draw(myBatch);
                        }
                    }
                }
            }
        }
    }
}
