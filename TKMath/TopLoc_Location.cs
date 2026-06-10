namespace TKMath
{
    //! A Location is a composite transition. It comprises a
    //! series of elementary reference coordinates, i.e.
    //! objects of type TopLoc_Datum3D, and the powers to
    //! which these objects are raised.

    public class TopLoc_Location
    {
        public TopLoc_SListOfItemLocation myItems = new TopLoc_SListOfItemLocation();

        public static implicit operator gp_Trsf(TopLoc_Location f)
        {
            return f.Transformation();
        }

        public TopLoc_Location()
        {
            myItems = new TopLoc_SListOfItemLocation();
        }

        public TopLoc_Location(gp_Trsf T)
        {
            TopLoc_Datum3D D = new TopLoc_Datum3D(T);
            myItems.Construct(new TopLoc_ItemLocation(D, 1));
        }
        

        public static double ScalePrec()
        {

            return 1e-14;

        }

        public void Clear()
        {
            myItems.Clear();
        }

        public TopLoc_Location Predivided(TopLoc_Location Other)

        {
            return Other.Inverted().Multiplied(this);
        }

        public static TopLoc_Location operator *(TopLoc_Location a, TopLoc_Location Other)
        {
            return a.Multiplied(Other);
        }

        private TopLoc_Location Multiplied(TopLoc_Location Other)
        {
            // prepend the chain Other in front of this
            // cancelling null exponents

            if (IsIdentity())
                return Other;
            if (Other.IsIdentity())
                return this;

            // prepend the queue of Other
            TopLoc_Location result = Multiplied(Other.NextLocation());
            // does the head of Other cancel the head of result

            int p = Other.FirstPower();
            if (!result.IsIdentity())
            {
                if (Other.FirstDatum() == result.FirstDatum())
                {
                    p += result.FirstPower();
                    result.myItems.ToTail();
                }
            }
            if (p != 0)
                result.myItems.Construct(new TopLoc_ItemLocation(Other.FirstDatum(), p));
            return result;

        }

        private TopLoc_Location NextLocation()
        {
            //????
            throw new NotImplementedException();
            //return new  TopLoc_Location( myItems.Tail());            //not origin code
        }

        private int FirstPower()
        {
            throw new NotImplementedException();
        }

        private TopLoc_Datum3D FirstDatum()
        {
            throw new NotImplementedException();
        }

        public TopLoc_Location Inverted()
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

        public bool IsIdentity()
        {
            return myItems.IsEmpty();
        }

        static readonly gp_Trsf TheIdentity = new gp_Trsf();

        public gp_Trsf Transformation()
        {

            if (IsIdentity())
                return TheIdentity;
            else
                return myItems.Value().myTrsf;

        }

        public void Identity()
        {
            myItems.Clear();
        }

        public bool IsEqual(TopLoc_Location Other)
        {
            if (myItems == Other.myItems)
            {
                return true;
            }
            if (myItems.Value() == Other.myItems.Value())
                return true;

            //const void** p = (const void**) &myItems;
            //const void** q = (const void**) &Other.myItems;
            //if (*p == *q) { return Standard_True; }
            if (IsIdentity() || Other.IsIdentity()) { return false; }
            if (FirstDatum() != Other.FirstDatum()) { return false; }
            if (FirstPower() != Other.FirstPower()) { return false; }
            else { return NextLocation() == Other.NextLocation(); }


        }
    }
}
