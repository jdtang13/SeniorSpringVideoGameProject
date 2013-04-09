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
    public class WorldMapComponent : Component
    {

        List<NodeComponent> nodeList = new List<NodeComponent>();
        public void AddNode(NodeComponent myNode)
        {
            nodeList.Add(myNode);
        }
        

        NodeComponent selectedNode;
        public void SetSelectedNode(NodeComponent myNode)
        {
            selectedNode = myNode;
        }

        public WorldMapComponent()
        {
            this.name = "WorldMapComponent";
        }

        //Uses the specificed node
        public void SelectNode(NodeComponent myNode)
        {
        }

        //Uses the currently selected node
        public void SelectNode()
        {

        }
    }
}
