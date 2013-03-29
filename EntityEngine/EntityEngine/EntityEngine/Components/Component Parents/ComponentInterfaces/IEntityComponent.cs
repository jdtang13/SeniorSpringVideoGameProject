using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityEngine.Components.Component_Parents.Entity_Bases
{
    public interface IEntityComponent
    {
        //DO NOT CHANGE

        string name
        {
            get;
        }

        void Initialize();

        void Start();
    }
}
