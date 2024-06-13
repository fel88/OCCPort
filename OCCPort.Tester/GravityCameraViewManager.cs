using OpenTK;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OCCPort.Tester
{
    public class GravityCameraViewManager : CameraViewManager
    {
        OCCPort.V3d_View view = new V3d_View();
        public OCCPort.V3d_View View => view;
        public override void Update()
        {
            var cc1 = view.Eye();
            var cc2 = view.At();
            var cc3 = view.Up().XYZ();
            Camera.CamFrom = new Vector3d(cc1.X(), cc1.Y(), cc1.Z());
            Camera.CamTo = new Vector3d(cc2.X(), cc2.Y(), cc2.Z());
            Camera.CamUp = new Vector3d(cc3.X(), cc3.Y(), cc3.Z());
            Camera.OrthoWidth = view.ViewX();
        }

        public float AlongRotate = 0;
        public Camera Camera;
        public override void Attach(EventWrapperGlControl control, Camera camera)
        {
            base.Attach(control, camera);
            Camera = camera;
            control.MouseUpAction = Control_MouseUp;
            control.MouseDownAction = Control_MouseDown;
            control.KeyUpUpAction = Control_KeyUp;
            control.KeyDownAction = Control_KeyDown;
            control.MouseWheelAction = Control_MouseWheel;
            control.MouseMoveAction = Control_MouseMove;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                view.Rotation(e.Location.X, e.Location.Y);
            }
            if (e.Button == MouseButtons.Left && isDrag)
            {
                var delta = new System.Drawing.Point(e.Location.X - startDrag.X, startDrag.Y - e.Location.Y);
                view.Pan(delta.X, delta.Y);
                startDrag = e.Location;
            }
            //view.MoveTo(e.Location.X, e.Location.Y);

        }
        Point startDrag;
        bool isDrag;
		GLControl glControl;
		public bool ZoomInPointMode = true;
        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
			if (ZoomInPointMode)
			{
				var p = e.Location;

				View.StartZoomAtPoint(p.X, p.Y);
				double delta = (double)(e.Delta) / (15 * 8);
				int x = p.X;
				int y = p.Y;
				int x1 = (int)(p.X + glControl.Width * delta / 100);
				int y1 = (int)(p.Y + glControl.Height * delta / 100);
				View.ZoomAtPoint(x, y, x1, y1);
			}
			else
            view.Zoom(0, 0, e.Delta / 8, 0);
            return;
            float zoomK = 20;
            var cur = Control.PointToClient(Cursor.Position);
            Control.MakeCurrent();
            //MouseRay.UpdateMatrices();
            MouseRay mr = new MouseRay(cur.X, cur.Y, Camera);
            //MouseRay mr0 = new MouseRay(Control.Width / 2, Control.Height / 2, Camera);

            var camera = Camera;
            if (camera.IsOrtho)
            {
                var shift = mr.Start - Camera.CamFrom;
                shift.Normalize();
                //var old = camera.OrthoWidth / Control.Width;
                if (e.Delta > 0)
                {
                    camera.OrthoWidth /= 1.2f;

                }
                else
                {
                    camera.OrthoWidth *= 1.2f;
                }
                /*var pxn = new Vector2(cur.X, cur.Y) - (new Vector2(Control.Width / 2, Control.Height / 2));

                var a1 = pxn * camera.OrthoWidth / Control.Width;*/
                Camera cam2 = new Camera();
                cam2.CamFrom = camera.CamFrom;
                cam2.CamTo = camera.CamTo;
                cam2.CamUp = camera.CamUp;
                cam2.OrthoWidth = camera.OrthoWidth;
                cam2.IsOrtho = camera.IsOrtho;

                cam2.UpdateMatricies(Control);
                MouseRay mr2 = new MouseRay(cur.X, cur.Y, cam2);

                var diff = mr.Start - mr2.Start;
                shift *= diff.Length;
                if (e.Delta > 0)
                {
                    camera.CamFrom += shift;
                    camera.CamTo += shift;
                }
                else
                {
                    camera.CamFrom -= shift;
                    camera.CamTo -= shift;
                }

                return;
            }
            if (
                Control.ClientRectangle.IntersectsWith(new Rectangle(Control.PointToClient(Cursor.Position),
                    new System.Drawing.Size(1, 1))))
            {
                var dir = mr.Dir;
                dir.Normalize();
                if (e.Delta > 0)
                {
                    camera.CamFrom += dir * zoomK;
                    camera.CamTo += dir * zoomK;
                }
                else
                {
                    camera.CamFrom -= dir * zoomK;
                    camera.CamTo -= dir * zoomK;
                }
            }
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                lshift = true;
            }
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            lshift = false;
        }

        protected bool lshiftcmd = false;
        public static Vector3d? lineIntersection(Vector3d planePoint, Vector3d planeNormal, Vector3d linePoint, Vector3d lineDirection)
        {
            if (Math.Abs(Vector3d.Dot(planeNormal, lineDirection)) < 10e-6f)
            {
                return null;
            }

            var dot1 = Vector3d.Dot(planeNormal, planePoint);
            var dot2 = Vector3d.Dot(planeNormal, linePoint);
            var dot3 = Vector3d.Dot(planeNormal, lineDirection);
            double t = (dot1 - dot2) / dot3;
            return linePoint + lineDirection * (float)t;
        }


        public virtual void Control_MouseDown(object sender, MouseEventArgs e)
        {
            //Control.MakeCurrent();
            startDrag = e.Location;
            if (e.Button == MouseButtons.Right)
            {
                view.StartRotation(e.Location.X, e.Location.Y);
            }

            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
            }
        }

        protected bool lshift = false;
        protected float startShiftX;
        protected float startShiftY;
        protected float startPosX;
        protected float startPosY;
        protected Vector3d cameraFromStart;
        protected Vector3d cameraToStart;
        protected Vector3d cameraUpStart;
        public PointF CursorPosition
        {
            get
            {
                return Control.PointToClient(Cursor.Position);
            }
        }
        protected bool drag = false;
        protected bool drag2 = false;

		public GravityCameraViewManager(GLControl glControl)
		{
			this.glControl = glControl;
		}

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
            if (e.Button == MouseButtons.Left)
            {
                //view.Select(ModifierKeys.HasFlag(Keys.Control));
                //SelectionChanged();
               // _currentTool.MouseUp(e);
            }
        }

		internal void FrontView()
		{
			view.FrontView();
		}
		internal void TopView()
		{
			view.TopView();
		}
	}
}
