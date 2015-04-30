using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CoordinateToGridExperiments.Renderers
{
    class ImageCoordinatePlaneRenderer : Renderer<CoordinatePlane, Image>
    {
        private static readonly StringFormat idFormat = new StringFormat(StringFormatFlags.FitBlackBox)
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
        };

        public override Image Render(CoordinatePlane plane, Size size)
        {
            var maxX = plane.Items.Max(x => x.Boundry.Right);
            var maxY = plane.Items.Max(x => x.Boundry.Bottom);
            var cellSize = new Size(size.Width / maxX, size.Height / maxY);

            var bitmap = new Bitmap(size.Width + 2, size.Height + 2);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                foreach (var item in plane.Items)
                {
                    var rectangle = Enlarge(item.Boundry, cellSize);

                    g.DrawRectangle(Pens.Black, rectangle);
                    g.DrawString(item.Component.Id.ToString(), SystemFonts.DefaultFont, Brushes.Black, rectangle, idFormat);
                }
            }

            return bitmap;
        }
    }
}
