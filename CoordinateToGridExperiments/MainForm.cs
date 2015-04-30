using CoordinateToGridExperiments.Renderers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoordinateToGridExperiments
{
    public partial class MainForm : Form
    {
        private readonly ImageGridRenderer imageRenderer = new ImageGridRenderer();
        private readonly BootstrapGridRenderer bootstrapRenderer = new BootstrapGridRenderer();
        private readonly ImageCoordinatePlaneRenderer coordinatePlaneRenderer = new ImageCoordinatePlaneRenderer();

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var coordinatePlane = CoordinatePlane.FromString(textBox1.Text);
            pictureBox1.Image = coordinatePlaneRenderer.Render(coordinatePlane, new Size(480, 360));

            var grid = coordinatePlane.ToGrid();
            pictureBox2.Image = imageRenderer.Render(grid, new Size(480, 360));

            textBox2.Text = this.bootstrapRenderer.Render(grid, new Size(12, 9));
        }
    }
}
