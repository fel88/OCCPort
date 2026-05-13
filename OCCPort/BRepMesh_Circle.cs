namespace OCCPort
{
    //! Describes a 2d circle with a size of only 3 Standard_Real 
    //! numbers instead of gp who needs 7 Standard_Real numbers.
    public class BRepMesh_Circle
    {
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