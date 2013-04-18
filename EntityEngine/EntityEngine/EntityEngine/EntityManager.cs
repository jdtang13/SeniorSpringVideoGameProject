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
        }

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

        public static void Draw(SpriteBatch myBatch,GraphicsDeviceManager myGraph)
        {
            //Cycle through the layers of all the entities, 0 being the most background
            for (int layer = 0; layer < LAYER_LIMIT; layer++)
            {
                myBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(myGraph.GraphicsDevice));
                
                for (int p = 0; p < masterList.Count; p++)
                {
                    if (masterList[p].layer == layer)
                    {
                        if (masterList[p].GetAssociatedState() == State.screenState)
                        {
                            masterList[p].Draw(myBatch);
                        }
                    }
                }
                myBatch.End();
            }
        }
    }
}
