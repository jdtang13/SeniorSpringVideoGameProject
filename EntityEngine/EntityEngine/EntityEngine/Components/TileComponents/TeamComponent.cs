using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;

namespace EntityEngine.Components.TileComponents
{
    public class TeamComponent : UpdateableComponent
    {
        List<Entity> unitEntityList = new List<Entity>();
        List<UnitComponent> unitCompList = new List<UnitComponent>();

        public void AddToTeam(Entity myEnt)
        {
            unitEntityList.Add(myEnt);
            unitCompList.Add(myEnt.getUpdateable("UnitComponent") as UnitComponent);
        }
        public void AddToTeam(UnitComponent myComp)
        {
            unitCompList.Add(myComp);
            unitEntityList.Add(myComp._parent);
        }
        
        public TeamComponent(Entity myParent)
            : base(myParent)
        {
            this.name = "UnitComponent";
        }        
    }
}
