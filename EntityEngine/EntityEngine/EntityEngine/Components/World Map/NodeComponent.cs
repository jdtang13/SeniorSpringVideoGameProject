using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Components.Component_Parents;
using Microsoft.Xna.Framework;
using EntityEngine.Components.TileComponents;
using EntityEngine.Components.Sprites;

namespace EntityEngine.Components.World_Map
{
    public class NodeComponent : Component
    {
        NodeState nodeState;
        public void SetNodeState(NodeState myState)
        {
            nodeState = myState;
        }
        public NodeState GetNodeState()
        {
            return nodeState;
        }

        string levelName;
        public string GetLevelName()
        {
            return levelName;
        }

        public NodeComponent(NodeState myNodeState, string myLevelName)
        {
            this.name = "NodeComponent";
            levelName = myLevelName;
            nodeState = myNodeState;
        }
    }
}
