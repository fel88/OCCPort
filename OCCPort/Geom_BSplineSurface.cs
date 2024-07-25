namespace OCCPort
{
	public class Geom_BSplineSurface
	{
		//=======================================================================
		//function : UDegree
		//purpose  : 
		//=======================================================================

		public int UDegree()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::UDegree");
		}


		//=======================================================================
		//function : NbVKnots
		//purpose  : 
		//=======================================================================

		public int NbVKnots()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbVKnots");
		}

		//=======================================================================
		//function : UDegree
		//purpose  : 
		//=======================================================================

		public int UDegree()
		{
			return udeg;
		}

		//=======================================================================
		//function : VDegree
		//purpose  : 
		//=======================================================================

		public int VDegree()
		{
			return vdeg;
		}

		//=======================================================================
		//function : NbUKnots
		//purpose  : 
		//=======================================================================

		public int NbUKnots()
		{
			return uknots.Length();
		}//=======================================================================
		 //function : UDegree
		 //purpose  : 
		 //=======================================================================

		public int UDegree()
		{
			return udeg;
		}
		GeomAbs_Shape Usmooth;
		GeomAbs_Shape Vsmooth;
		int udeg;
		int vdeg;
		//TColStd_HArray2OfReal weights;
		TColStd_HArray1OfReal ufknots;
		TColStd_HArray1OfReal vfknots;
		TColStd_HArray1OfReal uknots;
		TColStd_HArray1OfReal vknots;
	}
}