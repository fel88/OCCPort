using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCCPort.Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            glControl = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));




            if (glControl.Context.GraphicsMode.Samples == 0)
            {
                glControl = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            }
            evwrapper = new EventWrapperGlControl(glControl);

            glControl.Paint += Gl_Paint;
            ViewManager = GravityViewManager = new GravityCameraViewManager();
            ViewManager.Attach(evwrapper, camera1);

            Controls.Add(glControl);
            glControl.Dock = DockStyle.Fill;

        }

        GravityCameraViewManager GravityViewManager;
        public CameraViewManager ViewManager;
        Camera camera1 = new Camera() { IsOrtho = true };
        private EventWrapperGlControl evwrapper;
        GLControl glControl;

        private void Gl_Paint(object sender, PaintEventArgs e)
        {
            //if (!loaded)
            //  return;
            if (!glControl.Context.IsCurrent)
            {
                glControl.MakeCurrent();
            }


            Redraw();
        }

        void Redraw()
        {
            ViewManager.Update();

            GL.ClearColor(Color.LightGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            var o2 = Matrix4.CreateOrthographic(glControl.Width, glControl.Height, 1, 1000);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref o2);

            Matrix4 modelview2 = Matrix4.LookAt(0, 0, 70, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview2);



            GL.Enable(EnableCap.DepthTest);

            float zz = -500;
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.LightBlue);
            GL.Vertex3(-glControl.Width / 2, -glControl.Height / 2, zz);
            GL.Vertex3(glControl.Width / 2, -glControl.Height / 2, zz);
            GL.Color3(Color.AliceBlue);
            GL.Vertex3(glControl.Width / 2, glControl.Height / 2, zz);
            GL.Vertex3(-glControl.Width / 2, glControl.Height, zz);
            GL.End();
            GL.PushMatrix();
            GL.Translate(camera1.viewport[2] / 2 - 50, -camera1.viewport[3] / 2 + 50, 0);
            GL.Scale(0.5, 0.5, 0.5);

            var mtr = camera1.ViewMatrix;
            var q = mtr.ExtractRotation();
            var mtr3 = Matrix4d.CreateFromQuaternion(q);
            GL.MultMatrix(ref mtr3);
            GL.LineWidth(2);
            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);
            GL.End();

            GL.Color3(Color.Green);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 100, 0);
            GL.End();

            GL.Color3(Color.Blue);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 100);
            GL.End();
            GL.PopMatrix();
            camera1.Setup(glControl);

            if (true)
            {
                GL.LineWidth(2);
                GL.Color3(Color.Red);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(100, 0, 0);
                GL.End();

                GL.Color3(Color.Green);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 100, 0);
                GL.End();

                GL.Color3(Color.Blue);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, 100);
                GL.End();
            }

            GL.Enable(EnableCap.Light0);

            GL.ShadeModel(ShadingModel.Smooth);
            /*foreach (var item in Helpers)
            {
                item.Draw(null);
            }

            if (pickEnabled)
                PickUpdate();*/

            glControl.SwapBuffers();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            glControl.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GravityViewManager.View.StartRotation(glControl.Width / 2, glControl.Height / 2);
            GravityViewManager.View.Rotation(glControl.Width / 2, glControl.Height / 2 - 20);

        }

		private void frontToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GravityViewManager.FrontView();
		}

		private void topToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GravityViewManager.TopView();
		}
	}
}
