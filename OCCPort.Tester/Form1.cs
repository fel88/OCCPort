using AutoDialog;
using OCCPort.OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
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
            V3d_View.CreateView = () => new OpenGL.OpenGl_View();
            ViewManager = GravityViewManager = new GravityCameraViewManager(glControl);
            ViewManager.Attach(evwrapper, camera1);
            GravityViewManager.View.myView.Items.Add(new Graphic3d_MapOfStructure(
                new Graphic3d_Structure()
                {
                    myCStructure = new OpenGl_Structure()
                    {
                        visible = 1,
                        myBndBox = new Graphic3d_BndBox3d(new BVH_VecNt(0, 0, 0),
                             new BVH_VecNt(50, 50, 50))
                    }
                }));
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
            try
            {
                //GravityViewManager.View.myView.Redraw();
            }
            catch (Exception ex)
            {

            }
            Redraw();
        }

        void Redraw()
        {

            GravityViewManager.View.MyWindow.Width = glControl.Width;
            GravityViewManager.View.MyWindow.Height = glControl.Height;

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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddBoolField("zoomInPoint", "Zoom in point", GravityViewManager.ZoomInPointMode);
            if (!d.ShowDialog())
                return;

            GravityViewManager.ZoomInPointMode = d.GetBoolField("zoomInPoint");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            GravityViewManager.View.FrontView();
            var res1 = GravityViewManager.View.Convert(40);
            var d1 = GravityViewManager.View.Camera().Distance();





            GravityViewManager.View.Camera().SetScale(3000);
            var res2 = GravityViewManager.View.Convert(40);
            var d2 = GravityViewManager.View.Camera().Distance();
            GravityViewManager.View.Rotate(0, 0.5, 0, 0, 0, 0, true);
            var res3 = GravityViewManager.View.Convert(40);

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            GravityViewManager.View.FrontView();
            GravityViewManager.View.Camera().SetScale(1000);
            GravityViewManager.View.Rotate(0, 0.5, 0, 0, 0, 0, true);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var proxy = GravityViewManager.View;

            //proxy.Camera().SetScale(1000);
            proxy.FrontView();

            proxy.StartZoomAtPoint(glControl.Width / 2, glControl.Height / 3);
            var p = new System.Drawing.Point(0, 0);
            double delta = (double)(120) / (15 * 8);
            int x = p.X;
            int y = p.Y;
            int x1 = (int)(p.X + glControl.Width * delta / 100);
            int y1 = (int)(p.Y + glControl.Height * delta / 100);
            proxy.ZoomAtPoint(x, y, x1, y1);

            var res1 = proxy.Convert(40);

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            GravityViewManager.View.myView.Items.Clear();
        }
        AIS_InteractiveContext myAISContext;
        public void AddBox()
        {
            var d = DialogHelpers.StartDialog();
            d.Text = "New box";
            d.AddNumericField("w", "Width", 50);
            d.AddNumericField("l", "Length", 50);
            d.AddNumericField("h", "Height", 50);

            if (!d.ShowDialog())
                return;

            var w = d.GetNumericField("w");
            var h = d.GetNumericField("h");
            var l = d.GetNumericField("l");
            gp_Pnt p1 = new gp_Pnt(0, 0, 0);
            gp_Pnt p2 = new gp_Pnt(w, h, l);
            BRepPrimAPI_MakeBox box = new BRepPrimAPI_MakeBox(p1, p2);
            
            box.Build();
            var solid = box.Solid();
            var shape = new AIS_Shape(solid);
            myAISContext.Display(shape, true);
            myAISContext.SetDisplayMode(shape, AIS_DisplayMode. AIS_Shaded, false);
            //auto hn = GetHandle(*shape);
            //hh->FromObjHandle(hn);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            AddBox();
        }
    }

}
