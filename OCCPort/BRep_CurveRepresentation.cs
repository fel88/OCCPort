﻿namespace OCCPort
{
	//! a location.

	public class BRep_CurveRepresentation
	{
		public Geom_Curve Curve3D()
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}
		public virtual bool IsRegularity()
		{
			return false;
		}
		public bool IsCurveOnClosedSurface()
		{
			return false;
		}

		public virtual Geom2d_Curve PCurve2()
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}

		public virtual Geom2d_Curve PCurve()
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}

		//! A 3D curve representation.
		public virtual bool IsCurve3D()
		{
			return false;
		}

		public bool IsCurveOnSurface(Geom_Surface a, TopLoc_Location f)
		{
			return false;
		}

		//=======================================================================
		//function : Continuity
		//purpose  : 
		//=======================================================================

		public virtual GeomAbs_Shape Continuity()
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}

		public void Curve3D(Geom_Curve cc)
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}


		public bool IsCurveOnSurface()
		{
			return false;
		}

		public TopLoc_Location Location()
		{
			return myLocation;
		}
		//Standard_EXPORT BRep_CurveRepresentation(const TopLoc_Location& L);
		public void Location(TopLoc_Location L)
		{
			myLocation = L;
		}

		TopLoc_Location myLocation;


	}
}
