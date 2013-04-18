using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Components.Component_Parents.Entity_Bases
{
    public interface IEntityComponent
    {
        string name
        {
            get;
        }

        Entity Parent
        {
            get;
            set;
        }

        void Initialize();

        void Start();
    }
}
