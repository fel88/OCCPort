namespace TKMath
{
    //! Describes a coordinate transformation, i.e. a change
    //! to an elementary 3D coordinate system, or position in 3D space.
    //! A Datum3D is always described relative to the default datum.
    //! The default datum is described relative to itself: its
    //! origin is (0,0,0), and its axes are (1,0,0) (0,1,0) (0,0,1).
    public class TopLoc_Datum3D
    {
        public TopLoc_Datum3D(gp_Trsf aTrsf)
        {

            myTrsf = new gp_Trsf(aTrsf);
        }

        //! Returns a gp_Trsf which, when applied to this datum, produces the default datum.
        public gp_Trsf Transformation() { return myTrsf; }

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
