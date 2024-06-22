using OpenTK.Graphics.OpenGL;


namespace OCCPort.OpenGL
{
	public class _core11fwd
	{
		internal void glDisable(int v)
		{
			GL.Disable((EnableCap)v);
		}

		internal void glEnable(All v)
		{
			GL.Enable((EnableCap)v);
		}
	}
}