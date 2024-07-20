using OCCPort;
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

            if (IsIdentity()) return Other;
            if (Other.IsIdentity()) return this;

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
            throw new NotImplementedException();
        }

        private int FirstPower()
        {
            throw new NotImplementedException();
        }

        private TopLoc_Datum3D FirstDatum()
        {
            throw new NotImplementedException();
        }

        internal TopLoc_Location Clone()
        {
            return (TopLoc_Location)this.MemberwiseClone();

        }

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

        internal void Identity()
        {
            myItems.Clear();
        }
    }
}