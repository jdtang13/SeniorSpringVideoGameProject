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

        //Keeps a list of all the id's of the nodes it's connected to
        List<string> connectedTo = new List<string>();

        public NodeComponent(string myLevelID, string myLevelName,NodeState myNodeState,List<string> myConnectedTo)
        {
            this.name = "NodeComponent";
            levelName = myLevelName;
            nodeState = myNodeState;
        }
    }
}
