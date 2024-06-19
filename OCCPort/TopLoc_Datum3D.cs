using System;

namespace OCCPort
{
    public class TopLoc_Datum3D
    {
		public TopLoc_Datum3D(gp_Trsf aTrsf)
		{

			myTrsf = new gp_Trsf(aTrsf);
		}

		public TopLoc_Datum3D()
		{
		}

		gp_Trsf myTrsf;

		//! Returns a gp_Trsf which, when applied to this datum, produces the default datum.
		public gp_Trsf Trsf() { return myTrsf; }

		//! Return transformation form.
		public gp_TrsfForm Form() { return myTrsf.Form(); }

    }
}