using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;

        float deltaTime;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            MoveCursor();
            
            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();
        }
        void initialize()
        {
            renderer.Initialize();   
        }
        void MainLoop()
        {
            while (true)
            {
                renderer.Draw();
                renderer.Update(deltaTime);
                simpleOpenGlControl1.Refresh();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a')
                renderer.hero.Strafe(-1, renderer.skyboxSize);
            if (e.KeyChar == 'd')
                renderer.hero.Strafe(1, renderer.skyboxSize);
            if (e.KeyChar == 's')
                renderer.hero.Walk(-1, renderer.skyboxSize);
            if (e.KeyChar == 'w')
                renderer.hero.Walk(1, renderer.skyboxSize);
            if (e.KeyChar == 'z')
                renderer.hero.camera.Fly(-1, renderer.skyboxSize);
            if (e.KeyChar == 'c')
                renderer.hero.camera.Fly(1, renderer.skyboxSize);

            if (e.KeyChar == 'f')
                renderer.hero.Fire(renderer.bulletsList);
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 0.05f;
            float delta = e.X - prevX;
            if (delta > 2)
                renderer.hero.Yaw(-speed);
            else if (delta < -2)
                renderer.hero.Yaw(speed);


            delta = e.Y - prevY;
            if (delta > 2)
                renderer.hero.camera.Pitch(-speed);
            else if (delta < -2)
                renderer.hero.camera.Pitch(speed);

            MoveCursor();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            float r = float.Parse(textBox1.Text);
            float g = float.Parse(textBox2.Text);
            float b = float.Parse(textBox3.Text);
            float a = float.Parse(textBox4.Text);
            float s = float.Parse(textBox5.Text);
            renderer.SendLightData(r, g, b, a, s);
        }

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }
    }
}
