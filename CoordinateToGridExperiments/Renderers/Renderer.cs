using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CoordinateToGridExperiments.Renderers
{
    abstract class Renderer<T, TResult>
    {
        public abstract TResult Render(T input, Size size);

        protected static Rectangle Enlarge(Rectangle rect, Size size)
        {
            rect.X = rect.X * size.Width;
            rect.Y = rect.Y * size.Height;
            rect.Width = rect.Width * size.Width;
            rect.Height = rect.Height * size.Height;

            return rect;
        }

        protected static Rectangle Intersection(Rectangle rect, Rectangle rect2)
        {
            var l = Math.Max(rect.X, rect2.X);
            var t = Math.Max(rect.Y, rect2.Y);
            var r = Math.Min(rect.Right, rect2.Right);
            var b = Math.Min(rect.Bottom, rect2.Bottom);

            return Rectangle.FromLTRB(l, t, r, b);
        }
    }
}
