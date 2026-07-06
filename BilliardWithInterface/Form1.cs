namespace BilliardWithInterface
{
    public partial class Form1 : Form
    {

        double speedX = 2;
        double speedY = 10;
        double friction = 0.995;

        int mouseXStart = 0;
        int mouseYStart = 0;

        int mouseXCur = 0;
        int mouseYCur = 0;

        int mouseXEnd = 0;
        int mouseYEnd = 0;

        List<Point> lastPoint = new List<Point>();
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
        void PrintLabel1()
        {
            label1.Text = $"XStart: {mouseXStart:F2}, YStart: {mouseYStart:F2}, XCur: {mouseXCur:F2}, YCur: {mouseYCur:F2}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Left + speedX < 0)
            {
                speedX = -speedX;
                pictureBox1.Left = 0;
            }
            else if (pictureBox1.Left + speedX > 1000 - 131)
            {
                speedX = -speedX;
                pictureBox1.Left = 1000 - 131;
            }
            else
            {
                pictureBox1.Left += (int)speedX;
            }

            if (pictureBox1.Top + speedY < 0)
            {
                speedY = -speedY;
                pictureBox1.Top = 0;
            }
            else if (pictureBox1.Top + speedY > 500 - 131)
            {
                speedY = -speedY;
                pictureBox1.Top = 500 - 131;
            }
            else
            {
                pictureBox1.Top += (int)speedY;
            }

            speedX *= friction;
            speedY *= friction;

            lastPoint.Add(new Point(pictureBox1.Location.X, pictureBox1.Location.Y));
            if (lastPoint.Count()==100)
            {
                lastPoint.RemoveAt(0);
            }

            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red, 5);
            e.Graphics.DrawRectangle(pen, 0, 0, 1000, 500);

            // 2. DRAW THE TRACES (Only the last 5 points)
            int pointsToDraw = 5;

            // Start index is 5 items away from the end of the list (or 0 if the list is still small)
            int startIndex = Math.Max(0, lastPoint.Count - pointsToDraw);

            for (int i = startIndex; i < lastPoint.Count; i++)
            {
                // 'positionInTrail' will be 0 for the oldest drawn point, up to 4 for the newest drawn point
                int positionInTrail = i - startIndex;

                // Total points currently being drawn (usually 5, but less if game just started)
                int totalDrawn = lastPoint.Count - startIndex;

                // Calculate proportional opacity (Alpha) between 15 and 150 so it's a soft ghosting effect
                // Oldest point gets lowest opacity, newest point gets highest opacity
                int alpha = 15 + (int)(135 * ((double)(positionInTrail + 1) / totalDrawn));

                // Create a fading color brush (Matches your ball color, e.g., SlateGray)
                using (SolidBrush trailBrush = new SolidBrush(Color.FromArgb(alpha, Color.Aqua)))
                {
                    Point pt = lastPoint[i];
                    // Draw a circle matching the exact size of your 131x131 billiard ball
                    e.Graphics.FillEllipse(trailBrush, pt.X, pt.Y, 131, 131);
                }
            }

            // 3. Draw the aiming line
            int x = pictureBox1.Left + 131 / 2;
            int y = pictureBox1.Top + 131 / 2;
            e.Graphics.DrawLine(pen, x, y, mouseXCur, mouseYCur);

            pen.Dispose();


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Region = new System.Drawing.Region(path);

            List<PictureBox> pictureBoxClones = new List<PictureBox>();
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            for (int i = 0; i < lastPoint.Count; i++)
            {
                int alpha = (int)(255 * ((double)(i + 1) / lastPoint.Count)) / 3; 

                using (SolidBrush trailBrush = new SolidBrush(Color.FromArgb(alpha, Color.SlateGray)))
                {
                    Point pt = lastPoint[i];
                    e.Graphics.FillEllipse(trailBrush, pt.X, pt.Y, 131, 131);
                }
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
           mouseXEnd = e.X; mouseYEnd = e.Y; PrintLabel1();

            int x = pictureBox1.Left + 131 / 2;
            int y = pictureBox1.Top + 131 / 2;

            speedX = -(x - mouseXEnd) /10;
            speedY = -(y - mouseYEnd)/10;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseXCur = e.X; mouseYCur = e.Y; PrintLabel1();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseXStart = e.X; mouseYStart = e.Y; PrintLabel1();
        }
    }
}
