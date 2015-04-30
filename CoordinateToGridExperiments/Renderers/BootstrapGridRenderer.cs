using System.Drawing;
using System.Linq;
using System.Text;

namespace CoordinateToGridExperiments.Renderers
{
    class BootstrapGridRenderer :GridRenderer<string>
    {      
        private class Builder : IGridBuilder<string>
        {
            private StringBuilder builder;

            public void BeginGrid(Grid grid, Size size)
            {
                builder = new StringBuilder();
            }

            public void BeginRow(int index, Rectangle boundries)
            {
                builder.AppendLine("<div class=\"row row-md-" + boundries.Height + "\">");
            }

            public void BeginColumn(int index, Rectangle boundries)
            {
                builder.AppendLine("\t<div class=\"col-md-" + boundries.Width + "\">");
            }

            public void DrawInnerGrid(string innerGrid, Rectangle boundries)
            {
                builder.AppendLine("\t\t" + string.Join("\n\t\t", innerGrid.Split(new[] { '\n' })));
            }

            public void DrawComponent(Component component, Rectangle boundries)
            {
                builder.AppendLine("\t\t<div id=\"" + component.Id + "\" class=\"component\">" + component.Id + "</div>");
            }

            public void EndColumn(int index, Rectangle boundries)
            {
                builder.AppendLine("\t</div>");
            }

            public void EndRow(int index, Rectangle boundries)
            {
                builder.AppendLine("</div>");
            }

            public string EndGrid()
            {
                return builder.ToString();
            }

            public void Dispose()
            {

            }            
        }

        protected override bool UseSizeAsRatio
        {
            get
            {
                return true;
            }
        }

        protected override IGridBuilder<string> GenerateBuilder()
        {
            return new Builder();
        }        
    }
}
