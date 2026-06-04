using System.Reflection.Metadata;
using TKBRep;

namespace OCCPort
{

    //! The Curve2d from BRepAdaptor allows to use an Edge
    //! on   a Face like   a  2d   curve. (curve  in   the
    //! parametric space).
    //!
    //! It  has  the methods    of the class Curve2d  from
    //! Adpator.
    //!
    //! It  is created or  initialized with a  Face and an
    //! Edge.  The methods are  inherited from  Curve from
    //! Geom2dAdaptor.
    internal class BRepAdaptor_Curve2d : Geom2dAdaptor_Curve
    {

        TopoDS_Edge myEdge;
        TopoDS_Face myFace;
        public BRepAdaptor_Curve2d(TopoDS_Edge E,
                     TopoDS_Face F)
        {
            Initialize(E, F);
        }
        public void Initialize(TopoDS_Edge E,
                      TopoDS_Face F)
        {
            myEdge = E;
            myFace = F;
            double pf = 0, pl = 0;
            Geom2d_Curve PC = BRep_Tool.CurveOnSurface(E, F, ref pf, ref pl);
            base.Load(PC, pf, pl);
        }

    }
}