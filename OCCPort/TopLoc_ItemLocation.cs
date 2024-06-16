namespace OCCPort
{
    //! An ItemLocation is an elementary coordinate system
    //! in a Location.
    //!
    //! The  ItemLocation     contains :
    //!
    //! * The elementary Datum.
    //!
    //! * The exponent of the elementary Datum.
    //!
    //! * The transformation associated to the composition.
    
    public class TopLoc_ItemLocation
    {


        public TopLoc_Datum3D myDatum;
        public int myPower;
        public gp_Trsf myTrsf;
        

        public TopLoc_ItemLocation(TopLoc_Datum3D myDatum, int v)
        {
            this.myDatum = myDatum;
            this.myPower = v;
        }
    }
}