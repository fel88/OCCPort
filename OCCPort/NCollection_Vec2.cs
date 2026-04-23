using System;

namespace OCCPort
{
    public class NCollection_Vec2<T>
	{
		//! Per-component constructor.
		public NCollection_Vec2(T theX,

							  T theY)
		{

			v[0] = theX;
			v[1] = theY;
		}

		public NCollection_Vec2()
		{ 

		}
		//! Alias to 1st component as X coordinate in XY.
		public T x() { return v[0]; }

		//! Alias to 2nd component as Y coordinate in XY.
		public T y() { return v[1]; }

		T[] v = new T[2];
	}
}