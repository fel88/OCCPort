using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKernel;
using TKG3d;
using TKMath;

namespace TKMesh
{
    //! Interface class representing discrete model of a face.
    //! Face model contains one or several wires.
    //! First wire is always outer one.
    public interface IMeshData_Face : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Returns face's surface.
        BRepAdaptor_Surface GetSurface();
        TopoDS_Face GetFace();
        int WiresNb();

        //! Adds wire to discrete model of face.
        IWireHandle AddWire(TopoDS_Wire theWire, int theEdgeNb = 0);

        IWireHandle GetWire(int aWireIt);
    }
    //! Interface class representing shaped model with deflection.

    public interface IMeshData_TessellatedShape : IMeshData_Shape

    {
        //! Gets deflection value for the discrete model.
        double GetDeflection();
        //! Sets deflection value for the discrete model.
        void SetDeflection(double theValue);

        double myDeflection { get; set; }
    }

    //! Interface class representing model with associated TopoDS_Shape.
    //! Intended for inheritance by structures and algorithms keeping 
    //! reference TopoDS_Shape.
    public interface IMeshData_Shape
    {
        TopoDS_Shape GetShape();
        void SetShape(TopoDS_Shape shape);

    }
    //! Extension interface class providing status functionality.
    public interface IMeshData_StatusOwner
    {
        public IMeshData_Status GetStatusMask();

        //! Adds status to status flags of a face.
        public void SetStatus(IMeshData_Status theValue);

        bool IsSet(IMeshData_Status meshData_SelfIntersectingWire);
    }

    //! Interface class representing discrete model of a wire.
    //! Wire should represent an ordered set of edges.
    public interface IMeshData_Wire : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        int AddEdge(IMeshData_Edge theDEdge, TopAbs_Orientation theOrientation);

        int EdgesNb();
        IMeshData_Edge GetEdge(int aEdgeIt);
        TopAbs_Orientation GetEdgeOrientation(int aEdgeIt);

    }

    //! Interface class representing discrete model of an edge.
    public interface IMeshData_Edge : IMeshData_TessellatedShape, IMeshData_StatusOwner
    {  //! Sets 3d curve associated with current edge.
        void SetCurve(IMeshData_Curve theCurve);
        //! Returns 3d curve associated with current edge.
        IMeshData_Curve GetCurve();
        //! Gets value of angular deflection for the discrete model.
        double GetAngularDeflection();
        //! Updates same param flag.
        void SetSameParam(bool theValue);

        //! Returns degenerative flag.
        //! By default equals to flag stored in topological shape.
        bool GetDegenerated();
        //! Returns TopoDS_Edge attached to model.
        public TopoDS_Edge GetEdge()
        {
            return TopoDS.Edge(GetShape());
        }

        //! Sets value of angular deflection for the discrete model.
        public void SetAngularDeflection(double theValue);

        //! By default equals to flag stored in topological shape.
        public bool GetSameParam();

        //! Returns same range flag.
        //! By default equals to flag stored in topological shape.
        public bool GetSameRange();
        //! Returns number of pcurves assigned to current edge.
        public int PCurvesNb();
        IMeshData_PCurve GetPCurve(IMeshData_Face myDFace, TopAbs_Orientation topAbs_Orientation);
        //! Returns true in case if the edge is free one, i.e. it does not have pcurves.
        bool IsFree();
        //! Adds discrete pcurve for the specified discrete face.
        IMeshData_PCurve AddPCurve(
            IMeshData_Face theDFace,
            TopAbs_Orientation theOrientation);
        //! Returns pcurve with the given index.
        IMeshData_PCurve GetPCurve(int aPCurveIt);
        void SetDegenerated(bool v);
        void SetSameRange(bool v);

    }

    //! Enumerates statuses used to notify state of discrete model.
    public enum IMeshData_Status
    {
        IMeshData_NoError = 0x0,   //!< Mesh generation is successful.
        IMeshData_OpenWire = 0x1,   //!< Notifies open wire problem, which can potentially lead to incorrect results.
        IMeshData_SelfIntersectingWire = 0x2,   //!< Notifies self-intersections on discretized wire, which can potentially lead to incorrect results.
        IMeshData_Failure = 0x4,   //!< Failed to generate mesh for some faces.
        IMeshData_ReMesh = 0x8,   //!< Deflection of some edges has been decreased due to interference of discrete model.
        IMeshData_UnorientedWire = 0x10,  //!< Notifies bad orientation of a wire, which can potentially lead to incorrect results.
        IMeshData_TooFewPoints = 0x20,  //!< Discrete model contains too few boundary points to generate mesh.
        IMeshData_Outdated = 0x40,  //!< Existing triangulation of some faces corresponds to greater deflection than specified by parameter.
        IMeshData_Reused = 0x80,  //!< Existing triangulation of some faces is reused as far as it fits specified deflection.
        IMeshData_UserBreak = 0x100  //!< User break
    };

    //! Interface class representing discrete 3d curve of edge.
    //! Indexation of points starts from zero.
    public interface IMeshData_Curve : IMeshData_ParametersList, IParametersListPtrType
    {
        //! Returns discretization point with the given index.
        gp_Pnt GetPoint(int theIndex);
        //! Adds new discretization point to curve.
        void AddPoint(
   gp_Pnt thePoint,
   double theParamOnCurve);
        //! Inserts new discretization point at the given position.
        void InsertPoint(
   int thePosition,
   gp_Pnt thePoint,
   double theParamOnPCurve);

    }
    //! Interface class representing list of parameters on curve.
    public interface IMeshData_ParametersList : IParametersListPtrType
    {  //! Returns parameter with the given index.
        double GetParameter(int theIndex);

        //! Returns number of parameters.
        int ParametersNb();

        //! Clears parameters list.
        void Clear(bool isKeepEndPoints);

    }
    public interface IParametersListPtrType
    {
        double GetParameter(int theIndex);
        int ParametersNb();
    }
    //! Interface class representing pcurve of edge associated with discrete face.
    //! Indexation of points starts from zero.
    public interface IMeshData_PCurve : IMeshData_ParametersList
    {
        //! Returns orientation of the edge associated with current pcurve.
        TopAbs_Orientation GetOrientation();
        //! Returns forward flag of this pcurve.
        bool IsForward();

        int GetIndex(int theIndex);
        void SetIndex(int theIndex, int val);


        //! Returns discrete face pcurve is associated to.
        IMeshData_Face GetFace();
        //! Returns discretization point with the given index.
        gp_Pnt2d GetPoint(int theIndex);


        //! Adds new discretization point to pcurve.
        void AddPoint(gp_Pnt2d thePoint, double theParamOnPCurve);
        //! Inserts new discretization point at the given position.
        void InsertPoint(int thePosition, gp_Pnt2d thePoint, double theParamOnPCurve);

    }

    public abstract class AbstractMeshData_Face : AbstractMeshData_TessellatedShape, IMeshData_Face, IMeshData_StatusOwner
    {
        //! Constructor.
        //! Initializes empty model.
        public AbstractMeshData_Face(TopoDS_Face theFace)
         : base(theFace)
        {
            BRepAdaptor_Surface aSurfAdaptor = new BRepAdaptor_Surface(GetFace(), false);
            //mySurface = new BRepAdaptor_Surface(aSurfAdaptor);
            mySurface = aSurfAdaptor;//todo:Clone??
        }
        //=======================================================================
        public IWireHandle AddWire(
  TopoDS_Wire theWire,
  int theEdgeNb)
        {
            IWireHandle aWire = new BRepMeshData_Wire(theWire, theEdgeNb, myAllocator);
            myDWires.Append(aWire);
            return GetWire(WiresNb() - 1);
        }
        NCollection_IncAllocator myAllocator;

        VectorOfIWireHandles myDWires = new VectorOfIWireHandles();
        public int WiresNb()
        {
            return myDWires.Size();
        }
        //! Returns face's surface.
        public BRepAdaptor_Surface GetSurface()
        {
            return mySurface;
        }

        BRepAdaptor_Surface mySurface;
        //! Returns TopoDS_Face attached to model.
        public TopoDS_Face GetFace()
        {
            return TopoDS.Face(GetShape());
        }



        public IWireHandle GetWire(int theIndex)
        {
            return myDWires.Get(theIndex);
        }
    }

    public abstract class AbstractMeshData_TessellatedShape : AbstractMeshData_Shape, IMeshData_Shape, IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Constructor.
        public AbstractMeshData_TessellatedShape(TopoDS_Shape theShape)
     : base(theShape)

        {
            myDeflection = Standard_Real.RealLast();
        }

        //! Adds status to status flags of a face.
        public void SetStatus(IMeshData_Status theValue)
        {
            myStatus |= theValue;
        }

        //! Gets deflection value for the discrete model.
        public double GetDeflection()
        {
            return myDeflection;
        }
        //! Sets deflection value for the discrete model.
        public void SetDeflection(double theValue)
        {
            myDeflection = theValue;
        }

        IMeshData_Status myStatus;

        public bool IsSet(IMeshData_Status theValue)
        {
            return (myStatus & theValue) != 0;
        }

        public IMeshData_Status GetStatusMask()
        {
            return myStatus;
        }

        public double myDeflection { get; set; }
    }
    public abstract class AbstractMeshData_Shape : IMeshData_Shape
    {
        //! Returns shape assigned to discrete shape.
        public TopoDS_Shape GetShape()
        {
            return myShape;
        }
        TopoDS_Shape myShape;

        //! Assigns shape to discrete shape.
        public void SetShape(TopoDS_Shape theShape)
        {
            myShape = theShape;
        }

        //=======================================================================
        // Function: FacesNb
        // Purpose : 
        //=======================================================================
        public int FacesNb()
        {
            return myDFaces.Size();
        }

        protected VectorOfIFaceHandles myDFaces = new VectorOfIFaceHandles();
        //! Constructor.
        public AbstractMeshData_Shape()
        {
        }
        //! Constructor.
        public AbstractMeshData_Shape(TopoDS_Shape theShape)
        {
            myShape = (theShape);
        }

    }

    public class VectorOfIFaceHandles
    {
        //  IMeshData_Face
        List<IMeshData_Face> items = new List<IMeshData_Face>();
        public int Size()
        {
            return items.Count;
        }

        internal void Append(IMeshData_Face aEdge)
        {
            items.Add(aEdge);
        }

        internal IMeshData_Face Get(int v)
        {
            return items[v];
        }
    }

    //! Default implementation of edge data model entity.
    public class BRepMeshData_Wire : AbstractMeshData_Wire, IWireHandle
    {
        public override int EdgesNb()
        {
            return myDEdges.Count;
        }
        public override int AddEdge(IMeshData_Edge theDEdge, TopAbs_Orientation theOrientation)
        {
            int aIndex = EdgesNb();

            myDEdges.Add(theDEdge);
            myDEdgesOri.Add(theOrientation);

            return aIndex;
        }

        public override IMeshData_Edge GetEdge(int aEdgeIt)
        {
            return myDEdges[aEdgeIt];
        }
        //! Returns True if orientation of discrete edge with the given index is forward.

        public override TopAbs_Orientation GetEdgeOrientation(int theIndex)
        {
            return myDEdgesOri[theIndex];
        }

        VectorOfIEdgePtrs myDEdges;
        VectorOfOrientation myDEdgesOri;

        public BRepMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {

        }

        public BRepMeshData_Wire(TopoDS_Shape theShape, int theEdgeNb, NCollection_IncAllocator theAllocator) : this(theShape)
        {
            myDEdges = new VectorOfIEdgePtrs(theEdgeNb > 0 ? theEdgeNb : 256, theAllocator);
            myDEdgesOri = new VectorOfOrientation(theEdgeNb > 0 ? theEdgeNb : 256, theAllocator);
        }
    }

    public abstract class AbstractMeshData_Wire : AbstractMeshData_TessellatedShape, IMeshData_Wire
    {
        protected AbstractMeshData_Wire(TopoDS_Shape theShape) : base(theShape)
        {
        }

        public abstract int AddEdge(IMeshData_Edge theDEdge, TopAbs_Orientation theOrientation);

        public abstract int EdgesNb();

        public abstract IMeshData_Edge GetEdge(int aEdgeIt);
        public abstract TopAbs_Orientation GetEdgeOrientation(int aEdgeIt);
    }
    internal class VectorOfIEdgePtrs : List<IMeshData_Edge>
    {
        public VectorOfIEdgePtrs(int capacity, NCollection_IncAllocator theAllocator) : base(capacity)
        {
        }
    }

    public interface IMeshData_Model : IMeshData_Shape
    {
        //=======================================================================
        int FacesNb();
        IMeshData_Edge AddEdge(TopoDS_Edge theEdge);
        int EdgesNb();
        IMeshData_Face AddFace(TopoDS_Face theFace);
        IMeshData_Face GetFace(int theFaceIndex);
        IMeshData_Edge GetEdge(int v);

        //! Returns maximum size of shape model.
        double GetMaxSize();
    }
}

