using TKMath;

namespace OCCPort
{
    public class BRepClass_FaceClassifier : BRepClass_FClassifier
    {
        //! Classify  the Point  P  with  Tolerance <T> on the
        //! face described by <F>.
        //! Recommended to use Bnd_Box if the number of edges > 10
        //! and the geometry is mostly spline
        public void Perform(TopoDS_Face theF, gp_Pnt2d theP, double theTol,
                   bool theUseBndBox = false, double theGapCheckTol = 0.1)
        {
            BRepClass_FaceExplorer aFex = new BRepClass_FaceExplorer(theF);
            aFex.SetMaxTolerance(theGapCheckTol);
            aFex.SetUseBndBox(theUseBndBox);
            throw new Standard_NotImplemented();
            //base.Perform(aFex, theP, theTol);
        }

    }
}