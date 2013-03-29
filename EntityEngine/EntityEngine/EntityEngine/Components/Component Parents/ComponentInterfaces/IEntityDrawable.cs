using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngine.Components.Component_Parents.Entity_Bases
{
    public interface IEntityDrawable
    {
        //DONUT CHANGE

        bool visible
        {
            get;

        }

        void Draw(SpriteBatch batch);
    }
}
