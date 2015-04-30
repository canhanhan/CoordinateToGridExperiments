using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CoordinateToGridExperiments
{
    public class Component
    {
        public string Id { get; private set; }

        public Component(string id)
        {
            this.Id = id;
        }
    }
}
