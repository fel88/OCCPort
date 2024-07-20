namespace OCCPort
{
    public class Prs3d_LineAspect : Prs3d_BasicAspect
    {
        //! Returns the line aspect. This is defined as the set of
        //! color, type and thickness attributes.
        public Graphic3d_AspectLine3d Aspect() { return myAspect; }
        Graphic3d_AspectLine3d myAspect;


    }
}