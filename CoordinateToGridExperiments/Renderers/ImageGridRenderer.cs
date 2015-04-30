using System.Drawing;

namespace CoordinateToGridExperiments.Renderers
{
    class ImageGridRenderer : GridRenderer<Image>
    {
        private class Builder : IGridBuilder<Image>
        {
            private static readonly StringFormat rowFormat = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.DirectionVertical)
            {
                LineAlignment = StringAlignment.Near,
                Alignment = StringAlignment.Center
            };

            private static readonly StringFormat colFormat = new StringFormat(StringFormatFlags.FitBlackBox)
            {
                LineAlignment = StringAlignment.Far,
                Alignment = StringAlignment.Far
            };

            private static readonly StringFormat idFormat = new StringFormat(StringFormatFlags.FitBlackBox)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            private Bitmap bitmap;
            private Graphics g;

            public void BeginGrid(Grid grid, Size size)
            {
               bitmap = new Bitmap(size.Width + 2, size.Height + 2);
               g = Graphics.FromImage(bitmap);
               g.Clear(Color.White);
            }

            public void BeginRow(int index, Rectangle boundries)
            {               
            }

            public void BeginColumn(int index, Rectangle boundries)
            {
            }

            public void DrawInnerGrid(Image innerGrid, Rectangle boundries)
            {
                g.DrawImage(innerGrid, boundries.Location);
            }

            public void DrawComponent(Component component, Rectangle boundries)
            {
                g.DrawString(component.Id.ToString(), SystemFonts.DefaultFont, Brushes.Black, boundries, idFormat);
            }

            public void EndColumn(int index, Rectangle boundries)
            {
                g.DrawRectangle(Pens.Black, boundries);
                g.DrawString("Col " + index, SystemFonts.DefaultFont, Brushes.Black, boundries, colFormat);
            }

            public void EndRow(int index, Rectangle boundries)
            {
                g.DrawRectangle(Pens.Black, boundries);
                g.DrawString("Row " + index, SystemFonts.DefaultFont, Brushes.Black, boundries, rowFormat);
            }

            public Image EndGrid()
            {
                g.Flush();

                return (Image)bitmap.Clone();
            }

            public void Dispose()
            {
                if (g != null)
                    g.Dispose();

                if (bitmap != null)
                    bitmap.Dispose();
            }
        }

        protected override IGridBuilder<Image> GenerateBuilder()
        {
            return new Builder();
        }
    }
}
