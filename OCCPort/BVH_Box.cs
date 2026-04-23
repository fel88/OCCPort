namespace OCCPort
{
    public class BVH_Box
    {

        public

  BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?
                                   //! Returns minimum point of bounding box.
        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }


        //! Is bounding box valid?
        public bool IsValid() { return myIsInited; }
    }
}
