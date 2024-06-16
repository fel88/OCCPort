using System;
using System.Globalization;

namespace OCCPort
{
    //! A Location is a composite transition. It comprises a
    //! series of elementary reference coordinates, i.e.
    //! objects of type TopLoc_Datum3D, and the powers to
    //! which these objects are raised.

    public class TopLoc_Location
    {
        public TopLoc_Location()
        {
            myItems = new TopLoc_SListOfItemLocation();
        }
        internal static double ScalePrec()
        {
            
            return 1e-14;

        }

        public TopLoc_SListOfItemLocation myItems;

        internal TopLoc_Location Inverted()
        {

            //
            // the inverse of a Location is a chain in revert order
            // with opposite powers and same Local
            //
            TopLoc_Location result = new TopLoc_Location();
            TopLoc_SListOfItemLocation items = myItems;
            while (items.More())
            {
                result.myItems.Construct(new TopLoc_ItemLocation(items.Value().myDatum,
                                     -items.Value().myPower));
                items.Next();
            }
            return result;

        }

        internal bool IsIdentity()
        {
            return myItems.IsEmpty();
        }
        static readonly gp_Trsf TheIdentity = new gp_Trsf();

        internal gp_Trsf Transformation()
        {

            if (IsIdentity())
                return TheIdentity;
            else
                return myItems.Value().myTrsf;

        }
    }
}