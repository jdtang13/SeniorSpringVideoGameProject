using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityEngine.Components.Component_Parents.Entity_Bases;



namespace EntityEngine.Components.Component_Parents
{
    public class DrawableComponent : IEntityComponent, IEntityUpdateable, IEntityDrawable
    {
        //Parent of any component that needs to be drawn

        public Entity _parent;

        public Texture2D texture;
        public Vector2 offset, position, screenPosition;

        public string _name = "";
        public bool _enabled = true;
        public bool _visible = true;

        public int _updateOrder=0;

        public Boolean isMainSprite;

        public DrawableComponent(bool myMain)
        {
            isMainSprite = myMain;
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public bool visible
        {
            get
            {
                return _visible;
            }
        }
        public bool enabled
        {
            get
            {
                return _enabled;
            }
        }
        public int updateOrder
        {
            get
            {
                return _updateOrder;
            }
        }

        public Entity Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public virtual void Initialize()
        {
            

        }

        public virtual void Start()
        {

        }

        public virtual void LoadContent()
        {
            
        }

        public virtual void Update(GameTime myTime)
        {
            
        }

        public virtual void Draw(SpriteBatch batch)
        {
        }
    }


}
