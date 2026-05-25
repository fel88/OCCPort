namespace OCCPort
{
    public class BVH_Box<BVH_VecNt, MinMax> where BVH_VecNt : struct
          where MinMax : IBoxMinMax<BVH_VecNt>, new()
    {

        public BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?
                                   //! Returns minimum point of bounding box.

        MinMax minMax = new MinMax();

        //! Appends new point to the bounding box.
        public void Add(BVH_VecNt thePoint)
        {
            if (!myIsInited)
            {
                myMinPoint = thePoint;
                myMaxPoint = thePoint;
                myIsInited = true;
            }
            else
            {
                myMinPoint = minMax.CwiseMin(myMinPoint, thePoint);
                myMaxPoint = minMax.CwiseMax(myMaxPoint, thePoint);
            }
        }


        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }

        public void Combine(BVH_Box<BVH_VecNt, MinMax> theBox)
        {
            if (theBox.myIsInited)
            {
                if (!myIsInited)
                {
                    myMinPoint = theBox.myMinPoint;
                    myMaxPoint = theBox.myMaxPoint;
                    myIsInited = true;
                }
                else
                {
                    minMax.CwiseMin(ref myMinPoint, theBox.myMinPoint);
                    minMax.CwiseMax(ref myMaxPoint, theBox.myMaxPoint);
                }
            }
        }

        //! Is bounding box valid?
        public bool IsValid() { return myIsInited; }
    }
}
