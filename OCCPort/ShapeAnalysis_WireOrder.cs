using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using static OCCPort.Graphic3d_Camera;

namespace OCCPort
{

    //! This class is intended to control and, if possible, redefine
    //! the order of a list of edges which define a wire
    //! Edges are not given directly, but as their bounds (start,end)
    //!
    //! This allows to use this tool, either on existing wire, or on
    //! data just taken from a file (coordinates are easy to get)
    //!
    //! It can work, either in 2D, or in 3D, or miscible mode
    //! The tolerance for each mode is fixed
    //!
    //! Two phases : firstly add the couples (start, end)
    //! secondly perform then get the result
    internal class ShapeAnalysis_WireOrder
    {
        ModeType myMode;
        int[] myOrd;
        int[] myChains;
        int[] myCouples;
        List<gp_XYZ> myXYZ;
        List<gp_XY> myXY;
        double myTol;
        double myGap;
        int myStat;
        bool myKeepLoops;
        //! Sets new values.
        //! Clears the edge list if the mode (<theMode3D> or <theModeBoth> ) changes.
        //! Clears the connexion list.
        //! Warning: Parameter <theTolerance> is not used in algorithm.
        public void SetMode(bool theMode3D,
                                        double theTolerance,
                                        bool theModeBoth)
        {
            ModeType aNewMode;

            if (theModeBoth)
            {
                aNewMode = ModeType.ModeBoth;
            }
            else
            {
                if (theMode3D)
                {
                    aNewMode = ModeType.Mode3D;
                }
                else
                {
                    aNewMode = ModeType.Mode2D;
                }
            }
            if (myMode != aNewMode)
            {
                Clear();
            }
            myMode = aNewMode;
            myOrd = null;
            myStat = 0;
            myGap = 0.0;
            myTol = (theTolerance > 0.0) ? theTolerance : 1e-08;
        }

        //=======================================================================
        //function : Perform
        //purpose  : Make wire order analysis and propose the better order of the edges
        //           taking into account the gaps between edges.
        //=======================================================================
        public int NbEdges()
        {
            return myXYZ.Count / 2;
        }

        public int Ordered(int theIdx)
        {
            if (myOrd==null || myOrd.GetUpperBound(0) < theIdx)
                return theIdx;

            int anOldIdx = myOrd[theIdx];
            return (anOldIdx == 0 ? theIdx : anOldIdx);
        }

        public void Perform(bool closed)
        {
            myStat = 0;
            int aNbEdges = NbEdges();
            // no edges loaded, nothing to do -- return with status OK
            if (aNbEdges == 0)
            {
                return;
            }
            myOrd = new int[aNbEdges];
            //myOrd.Init(0);??

            // sequence of the edge nums in the right order
            List<int> anEdgeSeq = new List<int>();
            List<List<int>> aLoops = new List<List<int>>();

            // the beginnings and ends of the edges
            gp_XYZ[] aBegins3D = new gp_XYZ[aNbEdges];
            gp_XYZ[] anEnds3D = new gp_XYZ[aNbEdges];
            gp_XY[] aBegins2D = new gp_XY[aNbEdges];
            gp_XY[] anEnds2D = new gp_XY[aNbEdges];
            for (int i = 1; i <= aNbEdges; i++)
            {
                aBegins3D[i] = myXYZ[2 * i - 1];
                anEnds3D[i] = myXYZ[2 * i];
                if (myMode == ModeType.ModeBoth)
                {
                    aBegins2D[i] = myXY[2 * i - 1];
                    anEnds2D[i] = myXY[2 * i];
                }
            }

            // the flags that the edges was considered
            bool[] isEdgeUsed = new bool[aNbEdges];
            //isEdgeUsed.Init(false);

            double aTol2 = Precision.SquareConfusion();
            double aTolP2 = Precision.SquarePConfusion();

            // take the first edge to the constructed chain
            isEdgeUsed[1] = true;
            gp_Pnt aFirstPnt3D = new gp_Pnt(aBegins3D[1]);
            gp_Pnt aLastPnt3D = new gp_Pnt(anEnds3D[1]);
            gp_Pnt2d aFirstPnt2D = new gp_Pnt2d();
            gp_Pnt2d aLastPnt2D = new gp_Pnt2d();
            if (myMode == ModeType.ModeBoth)
            {
                aFirstPnt2D = new gp_Pnt2d(aBegins2D[1]);
                aLastPnt2D = new gp_Pnt2d(anEnds2D[1]);
            }
            anEdgeSeq.Add(1);

            // cycle until all edges are considered
            for (; ; )
            {
                // joint type
                // 0 - the start of the best edge to the end of constructed sequence (nothing to do)
                // 1 - the end of the best edge to the start of constructed sequence (need move the edge)
                // 2 - the end of the best edge to the end of constructed sequence (need to reverse)
                // 3 - the start of the best edge to the start of constructed sequence (need to reverse and move the edge)
                int aBestJointType = 3;
                // the best minimum distance between constructed sequence and the best edge
                double aBestMin3D = Standard_Real.RealLast();
                // number of the best edge
                int aBestEdgeNum = 0;
                // the best edge was found
                bool isFound = false;
                bool isConnected = false;
                // loop to find the best edge among all the remaining
                for (int i = 1; i <= aNbEdges; i++)
                {
                    if (isEdgeUsed[i])
                    {
                        continue;
                    }

                    // find minimum distance and joint type for 3D and 2D (if necessary) modes
                    int aCurJointType;
                    double aCurMin;
                    // distance for four possible cases
                    double aSeqTailEdgeHead = aLastPnt3D.SquareDistance(aBegins3D[i]);
                    double aSeqTailEdgeTail = aLastPnt3D.SquareDistance(anEnds3D[i]);
                    double aSeqHeadEdgeTail = aFirstPnt3D.SquareDistance(anEnds3D[i]);
                    double aSeqHeadEdgeHead = aFirstPnt3D.SquareDistance(aBegins3D[i]);
                    // the best distances for joints with head and tail of sequence
                    double aMinDistToTail, aMinDistToHead;
                    int aTailJoinType, aHeadJointType;
                    if (aSeqTailEdgeHead <= aSeqTailEdgeTail)
                    {
                        aTailJoinType = 0;
                        aMinDistToTail = aSeqTailEdgeHead;
                    }
                    else
                    {
                        aTailJoinType = 2;
                        aMinDistToTail = aSeqTailEdgeTail;
                    }
                    if (aSeqHeadEdgeTail <= aSeqHeadEdgeHead)
                    {
                        aHeadJointType = 1;
                        aMinDistToHead = aSeqHeadEdgeTail;
                    }
                    else
                    {
                        aHeadJointType = 3;
                        aMinDistToHead = aSeqHeadEdgeHead;
                    }
                    // comparing the head and the tail cases
                    // if distances are close enough then we use rule for joint type: 0 < 1 < 2 < 3
                    if (Math.Abs(aMinDistToTail - aMinDistToHead) < aTol2)
                    {
                        if (aTailJoinType < aHeadJointType)
                        {
                            aCurJointType = aTailJoinType;
                            aCurMin = aMinDistToTail;
                        }
                        else
                        {
                            aCurJointType = aHeadJointType;
                            aCurMin = aMinDistToHead;
                        }
                    }
                    else
                    {
                        if (aMinDistToTail <= aMinDistToHead)
                        {
                            aCurJointType = aTailJoinType;
                            aCurMin = aMinDistToTail;
                        }
                        else
                        {
                            aCurJointType = aHeadJointType;
                            aCurMin = aMinDistToHead;
                        }
                    }
                    // update for the best values
                    if (myMode == ModeType.ModeBoth)
                    {
                        // distances in 2D
                        int aJointMask3D = 0, aJointMask2D = 0;
                        if (aSeqTailEdgeHead < aTol2)
                        {
                            aJointMask3D |= (1 << 0);
                        }
                        if (aSeqTailEdgeTail < aTol2)
                        {
                            aJointMask3D |= (1 << 2);
                        }
                        if (aSeqHeadEdgeTail < aTol2)
                        {
                            aJointMask3D |= (1 << 1);
                        }
                        if (aSeqHeadEdgeHead < aTol2)
                        {
                            aJointMask3D |= (1 << 3);
                        }
                        double aSeqTailEdgeHead2D = aLastPnt2D.SquareDistance(aBegins2D[i].To_gp_Pnt2d());
                        double aSeqTailEdgeTail2D = aLastPnt2D.SquareDistance(anEnds2D[i].To_gp_Pnt2d());
                        double aSeqHeadEdgeTail2D = aFirstPnt2D.SquareDistance(anEnds2D[i].To_gp_Pnt2d());
                        double aSeqHeadEdgeHead2D = aFirstPnt2D.SquareDistance(aBegins2D[i].To_gp_Pnt2d());
                        if (aSeqTailEdgeHead2D < aTolP2)
                        {
                            aJointMask2D |= (1 << 0);
                        }
                        if (aSeqTailEdgeTail2D < aTolP2)
                        {
                            aJointMask2D |= (1 << 2);
                        }
                        if (aSeqHeadEdgeTail2D < aTolP2)
                        {
                            aJointMask2D |= (1 << 1);
                        }
                        if (aSeqHeadEdgeHead2D < aTolP2)
                        {
                            aJointMask2D |= (1 << 3);
                        }
                        // new approche for detecting best edge connection, for all other cases used old 3D algorithm
                        int aFullMask = aJointMask3D & aJointMask2D;
                        if (aFullMask != 0)
                        {
                            // find the best current joint type
                            aCurJointType = 3;
                            for (int j = 0; j < 4; j++)
                            {
                                if ((aFullMask & (1 << j)) != 0)
                                {
                                    aCurJointType = j;
                                    break;
                                }
                            }
                            if (!isConnected || aCurJointType < aBestJointType)
                            {
                                isFound = true;
                                isConnected = true;
                                switch (aCurJointType)
                                {
                                    case 0:
                                        aBestMin3D = aSeqTailEdgeHead;
                                        break;
                                    case 1:
                                        aBestMin3D = aSeqHeadEdgeTail;
                                        break;
                                    case 2:
                                        aBestMin3D = aSeqTailEdgeTail;
                                        break;
                                    case 3:
                                        aBestMin3D = aSeqHeadEdgeHead;
                                        break;
                                }
                                aBestJointType = aCurJointType;
                                aBestEdgeNum = i;
                            }
                        }
                        // if there is still no connection, continue to use ald 3D algorithm
                        if (isConnected)
                        {
                            continue;
                        }
                    }
                    // if the best distance is still not reached (aBestMin3D > aTol2) or we found a better joint type
                    if (aBestMin3D > aTol2 || aCurJointType < aBestJointType)
                    {
                        // make a decision that this edge is good enough:
                        // - it gets the best distance but there is fabs(aCurMin3d - aBestMin3d) < aTol2 && (aCurJointType < aBestJointType) ?
                        // - it gets the best joint in some cases
                        if (aCurMin < aBestMin3D || ((aCurMin == aBestMin3D || aCurMin < aTol2) && (aCurJointType < aBestJointType)))
                        {
                            isFound = true;
                            aBestMin3D = aCurMin;
                            aBestJointType = aCurJointType;
                            aBestEdgeNum = i;
                        }
                    }
                }

                // check that we found edge for connecting
                if (isFound)
                {
                    // distance between first and last point in sequence
                    double aCloseDist = aFirstPnt3D.SquareDistance(aLastPnt3D);
                    // if it's better to insert the edge than to close the loop, just insert the edge according to joint type
                    if (aBestMin3D <= Standard_Real.RealSmall() || aBestMin3D < aCloseDist)
                    {
                        switch (aBestJointType)
                        {
                            case 0:
                                anEdgeSeq.Add(aBestEdgeNum);
                                aLastPnt3D = anEnds3D[aBestEdgeNum].To_gp_Pnt(); ;
                                break;
                            case 1:
                                anEdgeSeq.Prepend(aBestEdgeNum);
                                aFirstPnt3D = aBegins3D[aBestEdgeNum].To_gp_Pnt();
                                break;
                            case 2:
                                anEdgeSeq.Append(-aBestEdgeNum);
                                aLastPnt3D = aBegins3D[aBestEdgeNum].To_gp_Pnt();
                                break;
                            case 3:
                                anEdgeSeq.Prepend(-aBestEdgeNum);
                                aFirstPnt3D = anEnds3D[aBestEdgeNum].To_gp_Pnt();
                                break;
                        }
                        if (myMode == ModeType.ModeBoth)
                        {
                            switch (aBestJointType)
                            {
                                case 0:
                                    aLastPnt2D = anEnds2D[aBestEdgeNum].To_gp_Pnt2d();
                                    break;
                                case 1:
                                    aFirstPnt2D = aBegins2D[aBestEdgeNum].To_gp_Pnt2d();
                                    break;
                                case 2:
                                    aLastPnt2D = aBegins2D[aBestEdgeNum].To_gp_Pnt2d();
                                    break;
                                case 3:
                                    aFirstPnt2D = anEnds2D[aBestEdgeNum].To_gp_Pnt2d();
                                    break;
                            }
                        }
                    }
                    // closing loop and creating new one
                    else
                    {
                        aLoops.Append(anEdgeSeq);
                        anEdgeSeq = new List<int>();
                        aFirstPnt3D = aBegins3D[aBestEdgeNum].To_gp_Pnt();
                        aLastPnt3D = anEnds3D[aBestEdgeNum].To_gp_Pnt();
                        if (myMode == ModeType.ModeBoth)
                        {
                            aFirstPnt2D = aBegins2D[aBestEdgeNum].To_gp_Pnt2d();
                            aLastPnt2D = anEnds2D[aBestEdgeNum].To_gp_Pnt2d();
                        }
                        anEdgeSeq.Append(aBestEdgeNum);
                    }
                    // mark the edge as used
                    isEdgeUsed[aBestEdgeNum] = true;
                }
                else
                {
                    // the only condition under which we can't find an edge is when all edges are done
                    break;
                }
            }
            // append the last loop
            aLoops.Append(anEdgeSeq);

            // handling with constructed loops
            List<int> aMainLoop;
            if (myKeepLoops)
            {
                // keeping the loops, adding one after another.
                aMainLoop = new List<int>();
                for (int i = 1; i <= aLoops.Count; i++)
                {
                    List<int> aCurLoop = aLoops[i];
                    aMainLoop.AddRange(aCurLoop);
                }
            }
            else
            {
                // connecting loops
                aMainLoop = aLoops.First();
                aLoops.RemoveAt(1);
                while (aLoops.Count != 0)
                {
                    // iterate over all loops to find the closest one
                    double aMinDist1 = Standard_Real.RealLast();
                    int aLoopNum1 = 0;
                    int aCurLoopIt1 = 0;
                    bool aDirect1 = false;
                    int aMainLoopIt1 = 0;
                    for (int aLoopIt = 1; aLoopIt <= aLoops.Count; aLoopIt++)
                    {
                        var aCurLoop = aLoops.Value(aLoopIt);
                        // iterate over all gaps between edges in current loop
                        int aCurLoopIt2 = 0;
                        int aMainLoopIt2 = 0;
                        bool aDirect2 = false;
                        double aMinDist2 = Standard_Real.RealLast();
                        int aCurLoopLength = aCurLoop.Length();
                        for (int aCurEdgeIt = 1; aCurEdgeIt <= aCurLoopLength; aCurEdgeIt++)
                        {
                            // get the distance between the current edge and the previous edge taking into account the edge's orientation
                            int aPrevEdgeIt = aCurEdgeIt == 1 ? aCurLoopLength : aCurEdgeIt - 1;
                            int aCurEdgeIdx = aCurLoop.Value(aCurEdgeIt);
                            int aPrevEdgeIdx = aCurLoop.Value(aPrevEdgeIt);
                            gp_Pnt aCurLoopFirst = aCurEdgeIdx > 0 ? aBegins3D[aCurEdgeIdx].To_gp_Pnt() : anEnds3D[-aCurEdgeIdx].To_gp_Pnt();
                            gp_Pnt aCurLoopLast = aPrevEdgeIdx > 0 ? anEnds3D[aPrevEdgeIdx].To_gp_Pnt() : aBegins3D[-aPrevEdgeIdx].To_gp_Pnt();
                            // iterate over all gaps between edges in main loop
                            double aMinDist3 = Standard_Real.RealLast();
                            int aMainLoopIt3 = 0;
                            var aDirect3 = false;
                            var aMainLoopLength = aMainLoop.Count();
                            for (int aCurEdgeIt2 = 1; (aCurEdgeIt2 <= aMainLoopLength) && aMinDist3 != 0.0; aCurEdgeIt2++)
                            {
                                // get the distance between the current edge and the next edge taking into account the edge's orientation
                                var aNextEdgeIt2 = aCurEdgeIt2 == aMainLoopLength ? 1 : aCurEdgeIt2 + 1;
                                var aCurEdgeIdx2 = aMainLoop.Value(aCurEdgeIt2);
                                var aNextEdgeIdx2 = aMainLoop.Value(aNextEdgeIt2);
                                gp_Pnt aMainLoopFirst = (aCurEdgeIdx2 > 0 ? anEnds3D[aCurEdgeIdx2].To_gp_Pnt() : aBegins3D[-aCurEdgeIdx2].To_gp_Pnt());
                                gp_Pnt aMainLoopLast = (aNextEdgeIdx2 > 0 ? aBegins3D[aNextEdgeIdx2].To_gp_Pnt() : anEnds3D[-aNextEdgeIdx2].To_gp_Pnt());
                                // getting the sum of square distances if we try to sew the current loop with the main loop in current positions
                                double aDirectDist =
                                        aCurLoopFirst.SquareDistance(aMainLoopFirst) + aCurLoopLast.SquareDistance(aMainLoopLast);
                                double aReverseDist =
                                        aCurLoopFirst.SquareDistance(aMainLoopLast) + aCurLoopLast.SquareDistance(aMainLoopFirst);
                                // take the best result
                                double aJoinDist;
                                if ((aDirectDist < aTol2) || (aDirectDist < 2.0 * aReverseDist))
                                {
                                    aJoinDist = aDirectDist;
                                    aReverseDist = aDirectDist;
                                }
                                else
                                {
                                    aJoinDist = aReverseDist;
                                }
                                // check if we found a better distance
                                if (aJoinDist < aMinDist3 && Math.Abs(aMinDist3 - aJoinDist) > aTol2)
                                {
                                    aMinDist3 = aJoinDist;
                                    aDirect3 = (aDirectDist <= aReverseDist);
                                    aMainLoopIt3 = aCurEdgeIt2;
                                }
                            }
                            // check if we found a better distance
                            if (aMinDist3 < aMinDist2 && Math.Abs(aMinDist2 - aMinDist3) > aTol2)
                            {
                                aMinDist2 = aMinDist3;
                                aDirect2 = aDirect3;
                                aMainLoopIt2 = aMainLoopIt3;
                                aCurLoopIt2 = aCurEdgeIt;
                            }
                        }
                        // check if we found a better distance
                        if (aMinDist2 < aMinDist1 && Math.Abs(aMinDist1 - aMinDist2) > aTol2)
                        {
                            aMinDist1 = aMinDist2;
                            aLoopNum1 = aLoopIt;
                            aDirect1 = aDirect2;
                            aMainLoopIt1 = aMainLoopIt2;
                            aCurLoopIt1 = aCurLoopIt2;
                        }
                    }
                    // insert the found loop into main loop
                    var aLoop = aLoops[aLoopNum1];
                    int aFactor = (aDirect1 ? 1 : -1);
                    for (int i = 0; i < aLoop.Count; i++)
                    {
                        int anIdx = (aCurLoopIt1 + i > aLoop.Count ? aCurLoopIt1 + i - aLoop.Count :
                                                  aCurLoopIt1 + i);
                        aMainLoop.InsertAfter(aMainLoopIt1 + i, aLoop[anIdx] * aFactor);
                    }
                    aLoops.RemoveAt(aLoopNum1);
                }
            }

            // checking the new order of the edges
            //  0 - order is the same
            //  1 - some edges were reordered
            // -1 - some edges were reversed
            int aTempStatus = 0;
            for (int i = 1; i <= aMainLoop.Count(); i++)
            {
                if (i != aMainLoop[i] && aTempStatus >= 0)
                {
                    aTempStatus = (aMainLoop[i] > 0 ? 1 : -1);
                }
                myOrd.SetValue(i, aMainLoop[i]);
            }
            if (aTempStatus == 0)
            {
                myStat = aTempStatus;
                return;
            }
            else
            {
                // check if edges were only shifted in reverse or forward, not reordered
                bool isShiftReverse = true;
                bool isShiftForward = true;
                int aFirstIdx, aSecondIdx;
                int aLength = aMainLoop.Count;
                for (int i = 1; i <= aLength - 1; i++)
                {
                    aFirstIdx = aMainLoop.Value(i);
                    aSecondIdx = aMainLoop.Value(i + 1);
                    if (!(aSecondIdx - aFirstIdx == 1 || (aFirstIdx == aLength && aSecondIdx == 1)))
                    {
                        isShiftForward = false;
                    }
                    if (!(aFirstIdx - aSecondIdx == 1 || (aSecondIdx == aLength && aFirstIdx == 1)))
                    {
                        isShiftReverse = false;
                    }
                }
                aFirstIdx = aMainLoop.Value(aLength);
                aSecondIdx = aMainLoop.Value(1);
                if (!(aSecondIdx - aFirstIdx == 1 || (aFirstIdx == aLength && aSecondIdx == 1)))
                {
                    isShiftForward = false;
                }
                if (!(aFirstIdx - aSecondIdx == 1 || (aSecondIdx == aLength && aFirstIdx == 1)))
                {
                    isShiftReverse = false;
                }
                if (isShiftForward || isShiftReverse)
                {
                    aTempStatus = 3;
                }
                myStat = aTempStatus;
                return;
            }
        }

        public int Status()
        {
            return myStat;
        }


        void Clear()
        {
            myXYZ = new List<gp_XYZ>();
            myXY = new List<gp_XY>();
            myStat = 0;
            myGap = 0.0;
        }
        public void Add(gp_XYZ theStart3d,
                                   gp_XYZ theEnd3d,
                                   gp_XY theStart2d,
                                   gp_XY theEnd2d)
        {
            if (myMode == ModeType.ModeBoth)
            {
                myXYZ.Append(theStart3d);
                myXYZ.Append(theEnd3d);

                myXY.Append(theStart2d);
                myXY.Append(theEnd2d);
            }
        }
        public void Add(gp_XY theStart2d, gp_XY theEnd2d)
        {
            if (myMode == ModeType.Mode2D)
            {
                gp_XYZ val = new gp_XYZ();
                val.SetCoord(theStart2d.X(), theStart2d.Y(), 0.0);
                myXYZ.Append(val);
                val.SetCoord(theEnd2d.X(), theEnd2d.Y(), 0.0);
                myXYZ.Append(val);
            }
        }

        public void Add(gp_XYZ theStart3d, gp_XYZ theEnd3d)
        {
            if (myMode == ModeType.Mode3D)
            {
                myXYZ.Append(theStart3d);
                myXYZ.Append(theEnd3d);
            }
        }
    }

    public static class Extensions
    {
        public static gp_Dir To_gp_Dir(this gp_Vec z)
        {
            return new gp_Dir(z);
        }
        public static gp_Pnt To_gp_Pnt(this gp_XYZ z)
        {
            return new gp_Pnt(z);
        }
        public static gp_Pnt2d To_gp_Pnt2d(this gp_XY z)
        {
            return new gp_Pnt2d(z);
        }
        public static int Value(this List<int> s, int ind)
        {
            return s[ind - 1];
        }
        public static T Value<T>(this List<T> s, int ind)
        {
            return s[ind - 1];
        }
        public static void InsertAfter<T>(this List<T> s, int ind, T val)
        {
            s.Insert(ind - 1, val);
        }
        public static int Length<T>(this List<T> s)
        {
            return s.Count;
        }
    }

}