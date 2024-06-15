namespace OCCPort
{
    public abstract class Graphic3d_Group
    {
        Graphic3d_TransformPers myTrsfPers; //!< current transform persistence
        protected Graphic3d_Structure myStructure;     //!< pointer to the parent structure
        //Graphic3d_BndBox4f myBounds;        //!< bounding box
        bool myIsClosed;      //!< flag indicating closed volume

        public virtual void AddPrimitiveArray(Graphic3d_TypeOfPrimitiveArray theType,
                                                Graphic3d_IndexBuffer theIndices,
                                                Graphic3d_Buffer theAttribs,
                                                Graphic3d_BoundBuffer theBounds,
                                                bool theToEvalMinMax)
        {

        }
    }
}