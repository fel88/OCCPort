namespace OCCPort
{
    //! Parameters for mouse scroll action.
    public struct Aspect_ScrollDelta
    {
        public Aspect_ScrollDelta()
        {
            Point = new NCollection_Vec2<int>();
        }
        public NCollection_Vec2<int> Point; //!< scale position
        public double Delta; //!< delta in pixels
        //public Aspect_VKeyFlags Flags; //!< key flags
        public int Flags; //!< key flags
    }

    

}
