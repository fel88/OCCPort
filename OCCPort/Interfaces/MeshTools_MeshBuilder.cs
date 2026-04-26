namespace OCCPort.Interfaces
{
    public class MeshTools_MeshBuilder : Message_Algorithm, IMeshTools_MeshBuilder
    {
        public MeshTools_MeshBuilder(IMeshTools_Context theContext)
        {
            myContext = theContext;
        }
        IMeshTools_Context myContext;

        //! Gets context of algorithm.
        IMeshTools_Context GetContext()
        {
            return myContext;
        }

        public void Perform(Message_ProgressRange theRange)
        {
            ClearStatus();

            IMeshTools_Context aContext = GetContext();
            if (aContext == null)
            {
                SetStatus(Message_Status.Message_Fail1);
                return;
            }

            Message_ProgressScope aPS = new Message_ProgressScope(theRange, "Mesh Perform", 10);

            if (aContext.BuildModel())
            {
                if (aContext.DiscretizeEdges())
                {
                    if (aContext.HealModel())
                    {
                        if (aContext.PreProcessModel())
                        {

                            if (aContext.DiscretizeFaces(aPS.Next(9)))
                            {
                                if (aContext.PostProcessModel())
                                {
                                    SetStatus(Message_Status.Message_Done1);
                                }

                                else
                                {
                                    SetStatus(Message_Status.Message_Fail7);
                                }
                            }

                            else
                            {
                                if (!aPS.More())
                                {
                                    SetStatus(Message_Status.Message_Fail8);
                                    aContext.Clean();
                                    return;
                                }
                                SetStatus(Message_Status.Message_Fail6);
                            }
                        }

                        else
                        {
                            SetStatus(Message_Status.Message_Fail5);
                        }
                    }

                    else
                    {
                        SetStatus(Message_Status.Message_Fail4);
                    }
                }

                else
                {
                    SetStatus(Message_Status.Message_Fail3);
                }
            }
            else
            {
                 IMeshTools_ModelBuilder aModelBuilder =
                  aContext.GetModelBuilder();

                 if (aModelBuilder == null)
                 {
                     SetStatus(Message_Status.Message_Fail1);
                 }
                 else
                 {
                     // Is null shape or another problem?
                     SetStatus(aModelBuilder.GetStatus().IsSet(Message_Status.Message_Fail1) ?
                       Message_Status.Message_Warn1 : Message_Status.Message_Fail2);
                 }
            }
            aPS.Next(1);
            	aContext.Clean();
        }

    }
}