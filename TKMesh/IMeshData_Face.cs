using OCCPort;
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
    public interface IParametersListPtrType
    {
        double GetParameter(int theIndex);
        int ParametersNb();
    }

    public class VectorOfIFaceHandles: NCollection_Vector<IMeshData_Face>
    {
        
    }
    
    internal class VectorOfOrientation : NCollection_Vector<TopAbs_Orientation> 
    {
        public VectorOfOrientation(int capacity, NCollection_IncAllocator theAllocator) : base(capacity,theAllocator)
        {
        }
    }

    //! Enumerates built-in meshing algorithms factories implementing IMeshTools_MeshAlgoFactory interface.
    public enum IMeshTools_MeshAlgoType
    {
        IMeshTools_MeshAlgoType_DEFAULT = -1, //!< use global default (IMeshTools_MeshAlgoType_Watson or CSF_MeshAlgo)
        IMeshTools_MeshAlgoType_Watson = 0,  //!< generate 2D Delaunay triangulation based on Watson algorithm (BRepMesh_MeshAlgoFactory)
        IMeshTools_MeshAlgoType_Delabella,    //!< generate 2D Delaunay triangulation based on Delabella algorithm (BRepMesh_DelabellaMeshAlgoFactory)
    };
    //! Base interface for factories producing instances of triangulation
    //! algorithms taking into account type of surface of target face.
    public interface IMeshTools_MeshAlgoFactory
    {
        //! Creates instance of meshing algorithm for the given type of surface.
        IMeshTools_MeshAlgo GetAlgo(
        GeomAbs_SurfaceType theSurfaceType,
        ref IMeshTools_Parameters theParameters);

    }
    //! Interface class providing API for algorithms intended to create mesh for discrete face.
    public interface IMeshTools_MeshAlgo
    {

        //! Performs processing of the given face.
        void Perform(
    IMeshData_Face theDFace,
    IMeshTools_Parameters theParameters,
    Message_ProgressRange theRange);

    }
}

