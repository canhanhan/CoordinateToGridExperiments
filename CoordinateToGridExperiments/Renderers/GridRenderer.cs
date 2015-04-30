using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CoordinateToGridExperiments.Renderers
{
    abstract class GridRenderer<T> : Renderer<Grid,T>
    {
        protected virtual bool UseSizeAsRatio { get { return false; } }

        public override T Render(Grid grid, Size size)
        {
            using (var renderer = GenerateBuilder())
            {
                renderer.BeginGrid(grid, size);

                var adjustedHeights = CalculateRowHeights(grid, ref size);
                for (var row = 0; row < grid.Rows.Count; row++)
                {
                    var rowBoundry = new Rectangle(0, adjustedHeights.Take(row).Sum(), size.Width, adjustedHeights[row]);
                    renderer.BeginRow(row, rowBoundry);

                    var adjustedWidths = CalculateColumnWidths(grid, ref size);
                    for (var column = 0; column < grid.Columns.Count; column++)
                    {
                        var columnBoundry = new Rectangle(adjustedWidths.Take(column).Sum(), 0, adjustedWidths[column], size.Height);
                        renderer.BeginColumn(column, columnBoundry);

                        var childBoundry = Intersection(rowBoundry, columnBoundry);
                        var item = grid[column, row];
                        if (item is Grid)
                        {
                            renderer.DrawInnerGrid(Render((Grid)item, UseSizeAsRatio ? size : childBoundry.Size), childBoundry);
                        }
                        else
                        {
                            renderer.DrawComponent((Component)item, childBoundry);
                        }

                        renderer.EndColumn(column, columnBoundry);
                    }

                    renderer.EndRow(row, rowBoundry);
                }

                return renderer.EndGrid();
            }
        }

        protected abstract IGridBuilder<T> GenerateBuilder();

        protected virtual int[] CalculateColumnWidths(Grid grid, ref Size size)
        {
            return Scale(grid.Columns.Select(col => col.Length), grid.Size.Width, size.Width);
        }

        protected virtual int[] CalculateRowHeights(Grid grid, ref Size size)
        {
            return Scale(grid.Rows.Select(row => row.Length), grid.Size.Height, size.Height);
        }

        protected static int[] Scale(IEnumerable<int> sizes, int currentSize, int desiredSize)
        {
            // Scale the current size to desired size
            var scaledSizes = sizes.Select(x => (double)x / currentSize * desiredSize).ToArray();           

            // Round the scaled sizes
            var roundedScaledSizes = scaledSizes.Select(x => (int)Math.Round(x, MidpointRounding.AwayFromZero));

            int diff;
            while ((diff = desiredSize - roundedScaledSizes.Sum()) != 0)
            {                
                double[] sizesToBeAdjusted;
                if (diff > 0)
                {
                    // Order rounded-down decimal numbers descending 
                    sizesToBeAdjusted = scaledSizes
                                        .Where(x => Math.Round(x, MidpointRounding.AwayFromZero) < x)
                                        .OrderByDescending(x => Math.Abs(x - Math.Round(x, MidpointRounding.AwayFromZero)))
                                        .ToArray();

                    // If there are not decimals, then start adjusting integers
                    if (sizesToBeAdjusted.Length == 0)
                        sizesToBeAdjusted = scaledSizes.OrderByDescending(x => x).ToArray();
                }
                else
                {
                    // Order rounded-up decimal numbers descending 
                    sizesToBeAdjusted = scaledSizes
                                        .Where(x => Math.Round(x, MidpointRounding.AwayFromZero) > x)
                                        .OrderByDescending(x => Math.Abs(x - Math.Round(x, MidpointRounding.AwayFromZero)))
                                        .ToArray();

                    // If there are not decimals, then start adjusting integers
                    if (sizesToBeAdjusted.Length == 0)
                        sizesToBeAdjusted = scaledSizes.OrderBy(x => x).ToArray();
                }

                for (var i = 0; i < sizesToBeAdjusted.Length && (diff = desiredSize - roundedScaledSizes.Sum()) != 0; i++)
                {
                    var size = sizesToBeAdjusted[i];
                    double newSize;

                    if (size == Math.Floor(size)) // if integer
                    {
                        newSize = diff > 0 ? size + 1 : size - 1;
                    }
                    else
                    {
                        newSize = diff > 0 ? Math.Ceiling(size) : Math.Floor(size);
                    }

                    scaledSizes[Array.IndexOf(scaledSizes, size)] = newSize;
                }
            }

            return roundedScaledSizes.ToArray();
        }
    }
}
