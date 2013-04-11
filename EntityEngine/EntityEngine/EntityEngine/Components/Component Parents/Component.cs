using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityEngine.Components.Component_Parents.Entity_Bases;

namespace EntityEngine.Components.Component_Parents
{
    public class Component : IEntityComponent
    {
        //Summary
        //Parents of any components that do not need to be updated or drawn.

        public string _name = "";
        public bool _enabled = true;

        public Entity _parent;

        public Component()
        {

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
    }


}
