namespace OCCPort
{
    //! Walking values.
    public struct AIS_WalkDelta
    {
        //! Empty constructor.
        public AIS_WalkDelta()
        {
            //: myIsDefined(false), myIsJumping(false), myIsCrouching(false), myIsRunning(false) 

        }
        AIS_WalkPart[] myTranslation = new AIS_WalkPart[3];
        AIS_WalkPart[] myRotation = new AIS_WalkPart[3];
        bool myIsDefined;
        bool myIsJumping;
        bool myIsCrouching;
        bool myIsRunning;
    }
}
