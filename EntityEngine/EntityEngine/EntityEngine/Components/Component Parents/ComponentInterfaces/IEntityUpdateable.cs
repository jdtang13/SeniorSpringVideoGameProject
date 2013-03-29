using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntityEngine.Components.Component_Parents.Entity_Bases
{
    public interface IEntityUpdateable
    {
        //DONUT CHANGE

        bool enabled
        {
            get;

        }
        int updateOrder
        {
            get;
        }


        void Update(GameTime gameTime);

    }
}
