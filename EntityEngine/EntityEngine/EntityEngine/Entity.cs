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

        //Each component is named in the constructor of the component. You call one of these methods method and pass the unitName string.
        //For instance, a sprite component is named "SpriteComponenent" in its code

        //So, if you wanted to use the sprite component of an entity named spaceShip(assuming it has one) the code would be:
        //        SpriteComponent spriteComp = spaceShip.GetDrawable("SpriteComponent") as SpriteComponent;

        //So since the function GetDrawable(string) returns a DrawableComponent (DrawableComponent is the parent of SpriteComponent) 
        //we have to use it AS a SpriteComponent

        
        public Component GetComponent(string myComponentName)
        {
            if (this.ComponentsDictionary.ContainsKey(myComponentName))
            {
                return ComponentsDictionary[myComponentName] as Component;
            }
            else
            {
                return null;
                //throw new ArgumentOutOfRangeException(myComponentName);
            }
        }
        public DrawableComponent GetDrawable(string myComponentName)
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
        public UpdateableComponent GetUpdateable(string myComponentName)
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

        public void AddComponent(IEntityComponent myComponent)
        {
            if (myComponent == null)
            {
                throw new ArgumentNullException("Componenet is null");
            }

            //take this code out if you want to have two of each component
            if (componentList.Contains(myComponent))
            {
                return;
            }

            //Setting the parent
            if (myComponent.Parent == null)
            {
                myComponent.Parent = this;
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
        //public int GetLayer()
        //{

        //}

        State.ScreenState screenState;
        public State.ScreenState GetAssociatedState()
        {
            return screenState;
        }

        public Entity(int myLayer, State.ScreenState myState)
        {
            layer = myLayer;
            screenState = myState;
        }

        public void Update(GameTime gameTime)
        {
            for (int p = 0; p < updateableComponentList.Count; p++)
            {
                if (updateableComponentList[p].enabled)
                    updateableComponentList[p].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            for (int p = 0; p < drawableComponentList.Count; p++)
            {
                if (drawableComponentList[p].visible)
                    drawableComponentList[p].Draw(batch);
            }
        }
    }
}
