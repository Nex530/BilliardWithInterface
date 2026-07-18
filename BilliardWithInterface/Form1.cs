namespace BilliardWithInterface
{
    public partial class Form1 : Form
    {

        double speedX = 2;
        double speedY = 10;
        double friction = 0.95;

        int mouseXStart = 0;
        int mouseYStart = 0;

        int mouseXCur = 0;
        int mouseYCur = 0;

        int mouseXEnd = 0;
        int mouseYEnd = 0;

        int score = 0;
        int clickCount = 0;

        Image originalImage;
        float rotationAngle = 0f;

        List<Point> lastPoint = new List<Point>();
        public Form1()
        {

            InitializeComponent();
            this.DoubleBuffered = true;
        }
        void PrintLabel1()
        {
            label1.Text = $"XStart: {mouseXStart:F2}, YStart: {mouseYStart:F2}, XCur: {mouseXCur:F2}, YCur: {mouseYCur:F2}, speedX: {speedX}, speedY: {speedY}\n" +
                $"score: {score}, clicks: {clickCount}";
        }

        void checkIfScore() 
        {
            int xB = pictureBox1.Location.X + pictureBox1.Width / 2;
            int xH = pictureHole.Location.X + pictureHole.Width / 2;
            int yB = pictureBox1.Location.Y + pictureBox1.Height / 2;
            int yH = pictureHole.Location.Y + pictureHole.Height / 2;

            bool canScore = Math.Sqrt(Math.Pow(xB - xH, 2) + Math.Pow(yB - yH, 2)) <= (pictureHole.Width / 2)? true : false;
      
            if (canScore) {
                score++;
              
                pictureHole.Location = new Point(new Random().Next(0, this.Width - pictureHole.Width-200), new Random().Next(0, this.Height - pictureHole.Height-200));
                speedX = 0;
                speedY = 0;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Left + speedX < 0)
            {
                speedX = -speedX;
                pictureBox1.Left = 0;
            }
            else if (pictureBox1.Left + speedX > this.Width- 20 - 40)
            {
                speedX = -speedX;
                pictureBox1.Left = this.Width - 20 - 40;
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
            else if (pictureBox1.Top + speedY > this.Height- 40 - 40)
            {
                speedY = -speedY;
                pictureBox1.Top = this.Height - 40 - 40;
            }
            else
            {
                pictureBox1.Top += (int)speedY;
            }

           speedX *= friction;
            speedY *= friction;

            checkIfScore();



            if (Math.Abs(speedX) < 0.1)
            {
                speedX = 0;
            }
            if (Math.Abs(speedY) < 0.1)
            {
                speedY = 0;
            }


            lastPoint.Add(new Point(pictureBox1.Location.X, pictureBox1.Location.Y));
            if (lastPoint.Count() == 50)
            {
                lastPoint.RemoveAt(0);
            }

            // 1. Simply increase the angle here (no heavy image generation!)
            rotationAngle += 5f;
            if (rotationAngle >= 360f)
            {
                rotationAngle = 0f;
            }

            // 2. Force the form to redraw itself
            this.Invalidate();

            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red, 5);
            e.Graphics.DrawRectangle(pen, 0, 0, this.Width-20 , this.Height-40);

            int pointsToDraw = 5;

            int startIndex = Math.Max(0, lastPoint.Count - pointsToDraw);

            for (int i = startIndex; i < lastPoint.Count; i++)
            {
                int positionInTrail = i - startIndex;

                int totalDrawn = lastPoint.Count - startIndex;

                int alpha = 15 + (int)(135 * ((double)(positionInTrail + 1) / totalDrawn));

                using (SolidBrush trailBrush = new SolidBrush(Color.FromArgb(alpha, Color.Aqua)))
                {
                    Point pt = lastPoint[i];
                    e.Graphics.FillEllipse(trailBrush, pt.X, pt.Y, 40, 40);
                }
            }

            int x = pictureBox1.Left + 40 / 2;
            int y = pictureBox1.Top + 40 / 2;
            using (Pen transparentPen = new Pen(Color.FromArgb(128, Color.Red), 5))
            {


                e.Graphics.DrawLine(transparentPen, x, y, mouseXCur , mouseYCur);
            }

            pen.Dispose();



            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Region = new System.Drawing.Region(path);

            System.Drawing.Drawing2D.GraphicsPath path1 = new System.Drawing.Drawing2D.GraphicsPath();
            path1.AddEllipse(0, 0, pictureHole.Width, pictureHole.Height);
            pictureHole.Region = new System.Drawing.Region(path1);

            List<PictureBox> pictureBoxClones = new List<PictureBox>();
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            pictureBoxClones.Add(pictureBox1);
            for (int i = 0; i < lastPoint.Count; i++)
            {
                int alpha = (int)(255 * ((double)(i + 1) / lastPoint.Count)) / 30;

                using (SolidBrush trailBrush = new SolidBrush(Color.FromArgb(alpha, Color.SlateGray)))
                {
                    Point pt = lastPoint[i];
                    e.Graphics.FillEllipse(trailBrush, pt.X, pt.Y, 40, 40);
                }
            }

            if (originalImage != null)
            {
                // 1. Save the current state of the graphics canvas
                System.Drawing.Drawing2D.GraphicsState state = e.Graphics.Save();

                // 2. Find the center point where pictureBox2 is placed on your screen
                float axeCenterX = pictureBox2.Left + pictureBox2.Width / 2f;
                float axeCenterY = pictureBox2.Top + pictureBox2.Height / 2f;

                // 3. Shift the canvas origin to the center of the axe
                e.Graphics.TranslateTransform(axeCenterX, axeCenterY);

                // 4. Apply your live rotation angle
                e.Graphics.RotateTransform(rotationAngle);

                // 5. Draw the image centered perfectly over our new origin
                // (We offset by negative half-width/height so it spins on its center axis)
                e.Graphics.DrawImage(originalImage,
                    -pictureBox2.Width / 2f,
                    -pictureBox2.Height / 2f,
                    pictureBox2.Width,
                    pictureBox2.Height);

                // 6. Restore the canvas state back to normal
                e.Graphics.Restore(state);
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originalImage = Properties.Resources.AXE2354__98622;

            if (originalImage != null)
            {
                pictureBox2.Visible = false; // Hide the control; we'll paint the image manually instead!
            }
        }

        private Bitmap GetRotatedImage(Image img, float angleInDegrees)
        {
            if (img == null) return null;

            // Create a blank bitmap matching the original size
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Set the pivot point to the absolute center of the image
                g.TranslateTransform(img.Width / 2f, img.Height / 2f);
                g.RotateTransform(angleInDegrees);
                g.TranslateTransform(-img.Width / 2f, -img.Height / 2f);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(img, new Point(0, 0));
            }
            return bmp;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
          if (speedX == 0 && speedY == 0)
            {
                clickCount++;
                mouseXEnd = e.X; mouseYEnd = e.Y; PrintLabel1();
                int x = pictureBox1.Left + 40 / 2;
                int y = pictureBox1.Top + 40 / 2;
                speedX = -(x - mouseXEnd)/25;
                speedY = -(y - mouseYEnd)/25;
            }
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
