using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace CoordinateToGridExperiments
{
    public class CoordinatePlane
    {
        [Serializable]
        public class CannotResolveGridException : Exception {}

        public class ComponentBoundryPair
        {
            public Component Component { get; private set; }
            public Rectangle Boundry { get; set; }

            public ComponentBoundryPair(Component component)
            {
                this.Component = component;
            }
        }

        private class CoordinateToGridConverter
        {
            public Grid Convert(IEnumerable<ComponentBoundryPair> components)
            {
                var grid = new Grid();

                DetectColumns(components, grid);
                DetectRows(components, grid);

                if (grid.Columns.Count == 1 && grid.Rows.Count == 1 && components.Any())
                {
                    throw new CannotResolveGridException();
                }

                for (var column = 0; column < grid.Columns.Count; column++)
                    for (var row = 0; row < grid.Rows.Count; row++)
                    {
                        var columnBoundry = grid.Columns[column];
                        var rowBoundry = grid.Rows[row];
                        var rect = new Rectangle(columnBoundry.Index, rowBoundry.Index, columnBoundry.Length, rowBoundry.Length);
                        var children = ObjectsIn(components, rect).ToArray();

                        if (children.Length > 1)
                        {
                            AdjustChildBoundries(children, rect);
                            grid[column, row] = Convert(children);
                        }
                        else if (children.Length == 1)
                        {
                            grid[column, row] = children.First().Component;
                        }
                    }

                return grid;
            }

            private static void DetectRows(IEnumerable<ComponentBoundryPair> components, Grid grid)
            {
                var maxY = components.Max(x => x.Boundry.Bottom);
                var rowBoundry = new Grid.Boundry();

                for (var y = 1; y <= maxY; y++)
                {
                    if (!CanSliceHorizontally(y, components))
                        continue;

                    rowBoundry.Length = y - rowBoundry.Index;
                    grid.AddRow(rowBoundry);

                    rowBoundry = new Grid.Boundry { Index = y };
                }
            }

            private static void DetectColumns(IEnumerable<ComponentBoundryPair> components, Grid grid)
            {
                var maxX = components.Max(x => x.Boundry.Right);
                var columnBoundry = new Grid.Boundry();

                for (var x = 1; x <= maxX; x++)
                {
                    if (!CanSliceVertically(x, components))
                        continue;

                    columnBoundry.Length = x - columnBoundry.Index;
                    grid.AddColumn(columnBoundry);

                    columnBoundry = new Grid.Boundry { Index = x };
                }
            }

            private static void AdjustChildBoundries(IEnumerable<ComponentBoundryPair> children, Rectangle parentRect)
            {
                foreach (var child in children)
                    child.Boundry = new Rectangle(new Point(child.Boundry.X - parentRect.X, child.Boundry.Y - parentRect.Y), child.Boundry.Size);
            }

            private static bool CanSliceVertically(int x, IEnumerable<ComponentBoundryPair> components)
            {
                return GetObjectsBeforeX(components, x).All(c => c.Boundry.Right <= x);
            }

            private static bool CanSliceHorizontally(int y, IEnumerable<ComponentBoundryPair> components)
            {
                return GetObjectsBeforeY(components,y).All(c => c.Boundry.Bottom <= y);
            }

            private static IEnumerable<ComponentBoundryPair> ObjectsIn(IEnumerable<ComponentBoundryPair> components, Rectangle rect)
            {
                return components.Where(c => c.Boundry.Top >= rect.Top && c.Boundry.Bottom <= rect.Bottom && c.Boundry.Left >= rect.Left && c.Boundry.Right <= rect.Right);
            }

            private static IEnumerable<ComponentBoundryPair> GetObjectsBeforeX(IEnumerable<ComponentBoundryPair> components, int x)
            {
                return components.Where(c => c.Boundry.Left < x);
            }

            private static IEnumerable<ComponentBoundryPair> GetObjectsBeforeY(IEnumerable<ComponentBoundryPair> components, int y)
            {
                return components.Where(c => c.Boundry.Top < y);
            }
        }

        private static Lazy<CoordinateToGridConverter> gridConverter = new Lazy<CoordinateToGridConverter>(() => new CoordinateToGridConverter());

        public static CoordinatePlane FromString(string text)
        {
            //id: x,y-WxH
            //id: x,y-w,y
            //0: 1,2-3x4
            var converter = TypeDescriptor.GetConverter(typeof(Point));
            return new CoordinatePlane(text.Replace(";", "\r\n").Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(line =>
                              {
                                  var parts = line.Split(new[] { ':' });
                                  var boundry = parts[1].Split(new[] { '-' })
                                                .Select(p => (Point)converter.ConvertFromInvariantString(p.Trim().Replace('x', ','))).ToArray();
                                  return new ComponentBoundryPair(new Component(parts[0]))
                                  {
                                      Boundry = new Rectangle(boundry.First().X, boundry.First().Y, boundry.Last().X, boundry.Last().Y)
                                  };
                              }).ToArray());
        }

        public IList<ComponentBoundryPair> Items { get; private set; }

        private CoordinatePlane(ComponentBoundryPair[] items)
        {
            Items = new List<ComponentBoundryPair>(items);
        }

        public CoordinatePlane()
        {
            Items = new List<ComponentBoundryPair>();
        }

        public Grid ToGrid()
        {
            return gridConverter.Value.Convert(Items);
        }
    }
}
