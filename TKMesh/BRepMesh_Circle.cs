using OCCPort.Common;
using TKMath;

namespace TKMesh
{
    //! Describes a 2d circle with a size of only 3 Standard_Real 
    //! numbers instead of gp who needs 7 Standard_Real numbers.
    public class BRepMesh_Circle
    {
        //! Default constructor.
        public BRepMesh_Circle()
        {
            myRadius = (0.0);
        }

        //! Sets radius of a circle.
        //! @param theRadius radius of a circle.
        public void SetRadius(double theRadius)
        {
            myRadius = theRadius;
        }

        //! Constructor.
        //! @param theLocation location of a circle.
        //! @param theRadius radius of a circle.
        public BRepMesh_Circle(gp_XY theLocation,
                    double theRadius)

        {
            myLocation = (theLocation);
            myRadius = (theRadius);
        }

        //! Returns radius of a circle.
        public double Radius()
        {
            return myRadius;
        }
        //! Returns location of a circle.
        public gp_XY Location()
        {
            return myLocation;
        }

        gp_XY myLocation;
        double myRadius;
    }
}

