using System;

namespace OCCPort
{
	//! This is a common interface for meshing algorithms 
	//! instantiated by Mesh Factory and implemented by plugins.
	public abstract class BRepMesh_DiscretRoot
	{
		//! Compute triangulation for set shape.
		public abstract void Perform(Message_ProgressRange theRange = null);

		
	}

}