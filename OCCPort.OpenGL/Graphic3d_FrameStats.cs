namespace OCCPort.OpenGL
{
    //! Class storing the frame statistics.
    public class Graphic3d_FrameStats
    {

        // =======================================================================
        // function : FrameEnd
        // purpose  :
        // =======================================================================
        public void FrameEnd(Graphic3d_CView theView,
                                       bool theIsImmediateOnly)
        {
            //Graphic3d_RenderingParams.PerfCounters aBits = !theView.IsNull()
            //                                                    ? theView->RenderingParams().CollectedStats
            //                                                    : Graphic3d_RenderingParams::PerfCounters_NONE;
            //if (theIsImmediateOnly
            // && (aBits & Graphic3d_RenderingParams::PerfCounters_SkipImmediate) != 0)
            //{
            //    return;
            //}

            //const double aTime = myFpsTimer.ElapsedTime();
            //myFrameDuration = aTime - myFrameStartTime;
            //++myFpsFrameCount;
            //if (!theView.IsNull())
            //{
            //    myUpdateInterval = theView->RenderingParams().StatsUpdateInterval;
            //}

            //if (aTime < myUpdateInterval)
            //{
            //    myCountersTmp.FlushTimers(myFpsFrameCount, false);
            //    return;
            //}

            //const Graphic3d_FrameStatsData&aPrevFrame = myCounters.Value(myLastFrameIndex);
            //if (aTime > gp::Resolution())
            //{
            //    // update FPS
            //    myFpsTimer.Stop();
            //    const double aCpuSec = myFpsTimer.UserTimeCPU();
            //    myCountersTmp[Graphic3d_FrameStatsTimer_ElapsedFrame] = aTime;
            //    myCountersTmp[Graphic3d_FrameStatsTimer_CpuFrame] = aCpuSec;

            //    if (theIsImmediateOnly)
            //    {
            //        myCountersTmp.ChangeImmediateFrameRate() = double(myFpsFrameCount) / aTime;
            //        myCountersTmp.ChangeImmediateFrameRateCpu() = aCpuSec > gp::Resolution()
            //                                                    ? double(myFpsFrameCount) / aCpuSec
            //                                                    : -1.0;
            //        myCountersTmp.ChangeFrameRate() = aPrevFrame.FrameRate();
            //        myCountersTmp.ChangeFrameRateCpu() = aPrevFrame.FrameRateCpu();
            //    }
            //    else
            //    {
            //        myCountersTmp.ChangeImmediateFrameRate() = -1.0;
            //        myCountersTmp.ChangeImmediateFrameRateCpu() = -1.0;
            //        myCountersTmp.ChangeFrameRate() = double(myFpsFrameCount) / aTime;
            //        myCountersTmp.ChangeFrameRateCpu() = aCpuSec > gp::Resolution()
            //                                           ? double(myFpsFrameCount) / aCpuSec
            //                                           : -1.0;
            //    }
            //    myCountersTmp.FlushTimers(myFpsFrameCount, true);
            //    myCountersMax.FillMax(myCountersTmp);
            //    myFpsTimer.Reset();
            //    myFpsTimer.Start();
            //    myFpsFrameCount = 0;
            //}

            //// update structure counters
            //if (theView.IsNull())
            //{
            //    myCounters.SetValue(myLastFrameIndex, myCountersTmp);
            //    myCountersTmp.Reset();
            //    return;
            //}

            //updateStatistics(theView, theIsImmediateOnly);

            //if (++myLastFrameIndex > myCounters.Upper())
            //{
            //    myLastFrameIndex = myCounters.Lower();
            //}
            //if (theIsImmediateOnly)
            //{
            //    // copy rendered counters collected for immediate layers
            //    const Standard_Integer anImmShift = Graphic3d_FrameStatsCounter_IMMEDIATE_LOWER - Graphic3d_FrameStatsCounter_RENDERED_LOWER;
            //    Standard_STATIC_ASSERT((Graphic3d_FrameStatsCounter_RENDERED_UPPER - Graphic3d_FrameStatsCounter_RENDERED_LOWER) == (Graphic3d_FrameStatsCounter_IMMEDIATE_UPPER - Graphic3d_FrameStatsCounter_IMMEDIATE_LOWER))
            //    for (Standard_Integer aCntIter = Graphic3d_FrameStatsCounter_RENDERED_LOWER; aCntIter <= Graphic3d_FrameStatsCounter_RENDERED_UPPER; ++aCntIter)
            //    {
            //        myCountersTmp.ChangeCounterValue((Graphic3d_FrameStatsCounter)(aCntIter + anImmShift)) = myCountersTmp.CounterValue((Graphic3d_FrameStatsCounter)aCntIter);
            //    }

            //    // copy main rendered counters from previous non-immediate frame
            //    for (Standard_Integer aCntIter = Graphic3d_FrameStatsCounter_RENDERED_LOWER; aCntIter <= Graphic3d_FrameStatsCounter_RENDERED_UPPER; ++aCntIter)
            //    {
            //        myCountersTmp.ChangeCounterValue((Graphic3d_FrameStatsCounter)aCntIter) = aPrevFrame.CounterValue((Graphic3d_FrameStatsCounter)aCntIter);
            //    }
            //    myCountersTmp.ChangeCounterValue(Graphic3d_FrameStatsCounter_EstimatedBytesGeom) = aPrevFrame.CounterValue(Graphic3d_FrameStatsCounter_EstimatedBytesGeom);
            //}
            //myCounters.SetValue(myLastFrameIndex, myCountersTmp);
            //myCountersTmp.Reset();
        }

    }
}