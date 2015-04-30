using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CoordinateToGridExperiments
{
    public class Grid
    {
        public struct Boundry
        {
            public int Index { get; set; }
            public int Length { get; set; } 
        }

        private readonly Dictionary<string, object> items = new Dictionary<string, object>();

        public Size Size { get; private set; }
        public List<Boundry> Rows { get; private set; }
        public List<Boundry> Columns { get; private set; }
        public IEnumerable<Component> Components { get { return this.items.Values.OfType<Component>().Concat(this.items.Values.OfType<Grid>().SelectMany(x => x.Components)); } }

        public Grid()
        {
            this.Rows = new List<Boundry>();
            this.Columns = new List<Boundry>();
        }

        public void AddRow(Boundry row)
        {
            this.Rows.Add(row);
            this.Size = new Size
            {
                Height = Rows.Max(x => x.Index + x.Length),
                Width = Size.Width
            };
        }

        public void AddColumn(Boundry column)
        {
            this.Columns.Add(column);
            this.Size = new Size
            {
                Height = Size.Height,
                Width = Columns.Max(x => x.Index + x.Length)
            };
        }

        public object this[int column, int row]
        {
            get
            {
                return this.items[column + "x" + row];
            }
            set
            {
                this.items[column + "x" + row] = value;
            }
        }
    }
}
