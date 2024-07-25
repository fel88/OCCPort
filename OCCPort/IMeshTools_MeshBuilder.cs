using OCCPort;
using System.Security.Cryptography;

namespace OCCPort
{
	public class IMeshTools_MeshBuilder : Message_Algorithm
	{
		public IMeshTools_MeshBuilder(IMeshTools_Context theContext)
		{
			myContext = (theContext);
		}
		IMeshTools_Context myContext;

		//! Gets context of algorithm.
		IMeshTools_Context GetContext()
		{
			return myContext;
		}

		public void Perform(Message_ProgressRange theRange)
		{
			/*ClearStatus();*/

			IMeshTools_Context aContext = GetContext();
			if (aContext == null)
			{
				SetStatus(Message_Status.Message_Fail1);
				return;
			}

			//Message_ProgressScope aPS(theRange, "Mesh Perform", 10);

			if (aContext.BuildModel())
			{
				/*if (aContext.kDiscretizeEdges())
				{
					if (aContext->HealModel())
					{
						if (aContext->PreProcessModel())
						{
							if (aContext->DiscretizeFaces(aPS.Next(9)))
							{
								if (aContext->PostProcessModel())
								{
									SetStatus(Message_Done1);
								}

								else
								{
									SetStatus(Message_Fail7);
								}
							}

							else
							{
								if (!aPS.More())
								{
									SetStatus(Message_Fail8);
									aContext->Clean();
									return;
								}
								SetStatus(Message_Fail6);
							}
						}

						else
						{
							SetStatus(Message_Fail5);
						}
					}

					else
					{
						SetStatus(Message_Fail4);
					}
				}

				else
				{
					SetStatus(Message_Fail3);
				}
			}
			else
			{
				IMeshTools_ModelBuilder aModelBuilder =
				 aContext.GetModelBuilder();

				if (aModelBuilder = null)
				{
					SetStatus(Message_Fail1);
				}
				else
				{
					// Is null shape or another problem?
					SetStatus(aModelBuilder->GetStatus().IsSet(Message_Fail1) ?
					  Message_Warn1 : Message_Fail2);
				}*/
			}
			//aPS.Next(1);
			//	aContext->Clean();
		}

	}
}