using System.Drawing;

namespace CoordinateToGridExperiments
{
    class ComponentBoundryPair
    {
        public Component Component { get; private set; }
        public Rectangle Boundry { get; set; }

        public ComponentBoundryPair(Component component)
        {
            this.Component = component;
        }
    }
}
