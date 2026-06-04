using TKernel;

namespace TKMesh
{
    //! Builds mesh for each face of shape without triangulation.
    //! In case if some faces of shape have already been triangulated
    //! checks deflection of existing polygonal model and re-uses it
    //! if deflection satisfies the specified parameter. Otherwise
    //! nullifies existing triangulation and build triangulation anew.
    //!
    //! The following statuses are used:
    //! Message_Done1 - algorithm has finished without errors.
    //! Message_Fail1 - invalid context.
    //! Message_Fail2 - algorithm has faced unexpected error.
    //! Message_Fail3 - fail to discretize edges.
    //! Message_Fail4 - can't heal discrete model.
    //! Message_Fail5 - fail to pre-process model.
    //! Message_Fail6 - fail to discretize faces.
    //! Message_Fail7 - fail to post-process model.
    //! Message_Warn1 - shape contains no objects to mesh.
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

        private void CheckEdges()
        {
            var edge0 = GetContext().GetModel().GetEdge(0);
            var edge1 = GetContext().GetModel().GetFace(0).GetWire(0).GetEdge(0);
            if (edge0 == edge1)
            {

            }
            var curve = edge1.GetCurve();

        }
    }
}

