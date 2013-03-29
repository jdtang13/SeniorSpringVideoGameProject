using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents.Entity_Bases;
using EntityEngine.Components.Component_Parents;


namespace EntityEngine
{
    public class Entity
    {
        public Dictionary<string, IEntityComponent> ComponentsDictionary = new Dictionary<string, IEntityComponent>();

        //At the start, an entity is a blank slate. In order to get it to do things, we add components to it. Components hold all
        //the functionality of an entity. For instance, a spaceship would have a spritecomponent, a physicscomponent, and a camera
        //component to describe it.
        
        //Each component is named in the constructor of the component. You call one of these methods method and pass the name string.
        //For instance, a sprite component is named "SpriteComponenent" in its code

        //So, if you wanted to use the sprite component of an entity named spaceShip(assuming it has one) the code would be:
        //        SpriteComponent spriteComp = spaceShip.getDrawable("SpriteComponent") as SpriteComponent;

        //So since the function getDrawable(string) returns a DrawableComponent (DrawableComponent is the parent of SpriteComponent) 
        //we have to use it AS a SpriteComponent

        public Component getComponent(string myComponentName)
        {
            if (this.ComponentsDictionary.ContainsKey(myComponentName))
            {
                return ComponentsDictionary[myComponentName] as Component;
            }
            else
            {
                throw new ArgumentOutOfRangeException(myComponentName);
            }
        }
        public DrawableComponent getDrawable(string myComponentName)
        {
            if (this.ComponentsDictionary.ContainsKey(myComponentName))
            {
                return ComponentsDictionary[myComponentName] as DrawableComponent;
            }
            else
            {
                DrawableComponent draw = null;
                return draw;
            }
        }
        public UpdateableComponent getUpdateable(string myComponentName)
        {
            if (this.ComponentsDictionary.ContainsKey(myComponentName))
            {
                return ComponentsDictionary[myComponentName] as UpdateableComponent;
            }
            else
            {
                throw new ArgumentOutOfRangeException(myComponentName);
            }
        }

        public List<IEntityComponent> componentList = new List<IEntityComponent>();
        public List<IEntityUpdateable> updateableComponentList = new List<IEntityUpdateable>();
        public List<IEntityDrawable> drawableComponentList = new List<IEntityDrawable>();

        private List<IEntityComponent> tempComponentList = new List<IEntityComponent>();
        private List<IEntityUpdateable> tempUpdateableComponentList = new List<IEntityUpdateable>();
        private List<IEntityDrawable> tempDrawableComponentList = new List<IEntityDrawable>();

        public void AddComponent(IEntityComponent myComponent)
        {
            if (myComponent == null)
            {
                throw new ArgumentNullException("Componenet is null");
            }

            if (componentList.Contains(myComponent))
            {
                return;
                //take this code out if you want to have two of each component
            }

            componentList.Add(myComponent);
            ComponentsDictionary.Add(myComponent.name, myComponent);

            IEntityUpdateable updateable = myComponent as IEntityUpdateable;
            IEntityDrawable drawable = myComponent as IEntityDrawable;

            if (updateable != null)
            {
                updateableComponentList.Add(updateable);
            }
            if (drawable != null)
            {
                drawableComponentList.Add(drawable);
            }

            myComponent.Initialize();
            myComponent.Start();

        }
        public bool RemoveComponent(IEntityComponent myComponent)
        {
            if (myComponent == null)
            {
                //throw a null exception
            }
            if (componentList.Remove(myComponent))
            {
                IEntityUpdateable updateable = myComponent as IEntityUpdateable;
                IEntityDrawable drawable = myComponent as IEntityDrawable;
                if (updateable != null)
                {
                    updateableComponentList.Remove(updateable);
                }
                if (drawable != null)
                {
                    drawableComponentList.Remove(drawable);
                }

                return true;
            }
            return false;
        }
        
        //Layer is used so that certain entities are drawn before others, background objects before foreground etc
        public int layer;

        //Some components have to update before others so we have an order system that can be set within the components
        //At max there are 100 different selections for priority for a component as detailed in this var
        private const int ORDER_THRESHOLD = 100;

        public Entity(int myLayer)
        {
            layer = myLayer;
        }

        public void Update(GameTime gameTime)
        {
            tempUpdateableComponentList.Clear();
            tempUpdateableComponentList.AddRange(updateableComponentList);

            //Cycle through the differnt update priorities, 0 being the msot importatn to update
            for (int q = 0; q < ORDER_THRESHOLD; q++)
            {
                for (int p = 0; p < tempUpdateableComponentList.Count; p++)
                {
                    //Check to see if it is indeed enabled and if it matches the priority
                    if (tempUpdateableComponentList[p].enabled && tempUpdateableComponentList[p].updateOrder == q)
                        tempUpdateableComponentList[p].Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            tempDrawableComponentList.Clear();
            tempDrawableComponentList.AddRange(drawableComponentList);

            for (int p = 0; p < tempDrawableComponentList.Count; p++)
            {
                if (tempDrawableComponentList[p].visible)
                    tempDrawableComponentList[p].Draw(batch);
            }
        }
    }
}
