namespace OCCPort
{
    public class BVH_Box
    {

        public BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?
                                   //! Returns minimum point of bounding box.
        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }

        public void Combine(BVH_Box theBox)
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
                    BoxMinMax.CwiseMin(myMinPoint, theBox.myMinPoint);
                    BoxMinMax.CwiseMax(myMaxPoint, theBox.myMaxPoint);
                }
            }
        }

        //! Is bounding box valid?
        public bool IsValid() { return myIsInited; }
    }
}
