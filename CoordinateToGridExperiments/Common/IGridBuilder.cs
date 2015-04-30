using System;
using System.Drawing;

namespace CoordinateToGridExperiments.Renderers
{
    interface IGridBuilder<T> : IDisposable
    {
        void BeginGrid(Grid grid, Size size);
        void BeginRow(int index, Rectangle boundries);
        void BeginColumn(int index, Rectangle boundries);
        void DrawInnerGrid(T innerGrid, Rectangle boundries);
        void DrawComponent(Component component, Rectangle boundries);
        void EndColumn(int index, Rectangle boundries);
        void EndRow(int index, Rectangle boundries);
        T EndGrid();
    }
}
