namespace OCCPort
{
	public class BRepMesh_DiscretFactory : BRepMesh_DiscretRoot
	{
		public static int A = 3;
		public static BRepMesh_DiscretRoot Discret(
  TopoDS_Shape theShape,
  double theDeflection,
  double theAngle)
		{
			BRepMesh_DiscretRoot aDiscretRoot;
			BRepMesh_DiscretRoot anInstancePtr = null;
			/*if (myPluginEntry != null)
			{
				// use plugin
				Standard_Integer anErr = myPluginEntry(theShape,
				  theDeflection, theAngle, anInstancePtr);

				if (anErr != 0 || anInstancePtr == NULL)
				{
					// can not create the algo specified - should never happens here
					myErrorStatus = BRepMesh_FE_CANNOTCREATEALGO;
					return aDiscretRoot;
				}
			}
			else //if (myDefaultName == THE_FAST_DISCRET_MESH)*/
			{
				// use built-in
				BRepMesh_IncrementalMesh.Discret(theShape,
				  theDeflection, theAngle, anInstancePtr);
			}

			// cover with handle
			aDiscretRoot = anInstancePtr;

			// return the handle
			return aDiscretRoot;
		}

		public override void Perform(Message_ProgressRange theRange = null)
		{
			throw new System.NotImplementedException();
		}
	}
}