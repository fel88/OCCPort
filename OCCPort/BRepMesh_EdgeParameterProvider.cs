using OCCPort.Interfaces;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Auxiliary class provides correct parameters 
    //! on curve regarding SameParameter flag.

    public class BRepMesh_EdgeParameterProvider  
    {
        //! Constructor. Initializes empty provider.
        public BRepMesh_EdgeParameterProvider()
        {
            myIsSameParam = (false);
            myFirstParam = (0.0);
            myOldFirstParam = (0.0);
            myScale = (0.0);
            myCurParam = (0.0);
            myFoundParam = 0.0;
        }

        //! Returns pcurve used to compute parameters.
        public Adaptor2d_Curve2d GetPCurve()
        {
            return myCurveAdaptor.CurveOnSurface().GetCurve();
        }

        //! Returns parameter according to SameParameter flag of the edge.
        //! If SameParameter is TRUE returns value from parameters w/o changes,
        //! elsewhere scales initial parameter and tries to determine resulting
        //! value using projection of the corresponded 3D point on PCurve.
        public double Parameter(int theIndex,
                          gp_Pnt thePoint3d)
        {
            if (myIsSameParam)
            {
                return myParameters.Value(theIndex);
            }

            // Use scaled
            double aParam = myParameters.Value(theIndex);

            double aPrevParam = myCurParam;
            myCurParam = myFirstParam + myScale * (aParam - myOldFirstParam);

            double aPrevFoundParam = myFoundParam;
            myFoundParam += (myCurParam - aPrevParam);

            myProjector.Perform(thePoint3d, myFoundParam);
            if (myProjector.IsDone())
            {
                double aFoundParam = myProjector.Point().Parameter();
                if ((aPrevFoundParam < myFoundParam && aPrevFoundParam < aFoundParam) ||
                    (aPrevFoundParam > myFoundParam && aPrevFoundParam > aFoundParam))
                {
                    // Rude protection against case when amplified parameter goes before 
                    // previous one due to period or other reason occurred in projector.
                    // Using parameter returned by projector as is can produce self-intersections.
                    myFoundParam = aFoundParam;
                }
            }

            return myFoundParam;
        }
        IParametersCollection myParameters;

        //! Constructor.
        //! @param theEdge edge which parameters should be processed.
        //! @param theFace face the parametric values are defined for.
        //! @param theParameters parameters corresponded to discretization points.
        public BRepMesh_EdgeParameterProvider(
    IMeshData_Edge theEdge,
    TopAbs_Orientation theOrientation,
IMeshData_Face theFace,
    IParametersCollection theParameters)
        {
            Init(theEdge, theOrientation, theFace, theParameters);
        }

        //! Initialized provider by the given data.
    public    void Init(
    IMeshData_Edge theEdge,
    TopAbs_Orientation theOrientation,
    IMeshData_Face theFace,
    IParametersCollection theParameters)
        {
            myParameters = theParameters;
            myIsSameParam = theEdge.GetSameParam();
            myScale = 1.0;

            // Extract actual parametric values
            TopoDS_Edge aEdge = TopoDS.Edge(theEdge.GetEdge().Oriented(theOrientation));

            myCurveAdaptor.Initialize(aEdge, theFace.GetFace());
            if (myIsSameParam)
            {
                return;
            }

            myFirstParam = myCurveAdaptor.FirstParameter();
            double aLastParam = myCurveAdaptor.LastParameter();

            myFoundParam = myCurParam = myFirstParam;

            // Extract parameters stored in polygon
            myOldFirstParam = myParameters.Value(myParameters.Lower());
            double aOldLastParam = myParameters.Value(myParameters.Upper());

            // Calculate scale factor between actual and stored parameters
            if ((myOldFirstParam != myFirstParam || aOldLastParam != aLastParam) &&
                myOldFirstParam != aOldLastParam)
            {
                myScale = (aLastParam - myFirstParam) / (aOldLastParam - myOldFirstParam);
            }

            myProjector.Initialize(myCurveAdaptor, myCurveAdaptor.FirstParameter(),
                                   myCurveAdaptor.LastParameter(), Precision.PConfusion());
        }


        bool myIsSameParam;
        double myFirstParam;

        double myOldFirstParam;
        double myScale;

        double myCurParam;
        double myFoundParam;

        BRepAdaptor_Curve myCurveAdaptor = new BRepAdaptor_Curve();

        Extrema_LocateExtPC myProjector = new Extrema_LocateExtPC();
    }
}