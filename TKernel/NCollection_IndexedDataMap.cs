using OCCPort;
using OCCPort.Common;
using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TKernel
{
    public class NCollection_IndexedDataMap<TheKeyType, T2> : NCollection_IndexedDataMap<TheKeyType, T2, NCollection_DefaultHasher<TheKeyType>>
    {

    }
    public class NCollection_IndexedDataMap<TheKeyType, T2, Hasher> : NCollection_BaseMap where Hasher : IEqualityComparer<TheKeyType>, new()
    {
        public void ChangeFromIndex(int theIndex, T2 v)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(), "NCollection_IndexedDataMap::ChangeFromIndex");
            var d = dic[theIndex - 1];
            dic[theIndex - 1] = new KeyValuePair<TheKeyType, T2>(d.Key, v);

        }
        public class Iterator
        {
            public Iterator(NCollection_IndexedDataMap<TheKeyType, T2, Hasher> aMapOfPCurves)
            {
                myMap = aMapOfPCurves;
                myIndex = 1;
            }
            NCollection_IndexedDataMap<TheKeyType, T2, Hasher> myMap;   //!< Pointer to current node
            int myIndex; //!< Current index

            //! Query if the end of collection is reached by iterator
            public bool More()
            {
                return myMap != null && myIndex <= myMap.Extent();
            }

            public TheKeyType Key()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_IndexedDataMap::Iterator::Key");
                return myMap.FindKey(myIndex);
            }

            //! Make a step along the collection
            public void Next()
            {
                ++myIndex;
            }
            //! Value access
            public T2 Value()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_IndexedDataMap::Iterator::Value");
                return myMap.FindFromIndex(myIndex);
            }



            public void ChangeValue(T2 v)
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_IndexedDataMap::Iterator::Value");
                myMap.dic[myIndex - 1] = new KeyValuePair<TheKeyType, T2>(myMap.dic[myIndex - 1].Key, v);
            }
        }

        public NCollection_IndexedDataMap()
        {
            dic = new List<KeyValuePair<TheKeyType, T2>>();
        }

        public void Clear()
        {
            dic.Clear();
        }

        public T2 ChangeFromKey(TheKeyType theKey1)
        {
            if (IsEmpty())
                return default;
            foreach (var item in dic)
            {
                if (hasher.Equals(item.Key, theKey1))
                    return item.Value;
            }
            return default;
        }

        //! Contains
        public bool Contains(TheKeyType theKey1)
        {
            if (IsEmpty())
                return false;
            foreach (var item in dic)
            {
                if (hasher.Equals(item.Key, theKey1))
                    return true;
            }
            return false;

            //Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* pNode1;
            //pNode1 = (IndexedDataMapNode*)myData1[iK1];
            //while (pNode1)
            //{
            //    if (Hasher::IsEqual(pNode1->Key1(), theKey1))
            //        return Standard_True;
            //    pNode1 = (IndexedDataMapNode*)pNode1->Next();
            //}
            //return Standard_False;
        }
        public List<KeyValuePair<TheKeyType, T2>> dic = null;

        Hasher hasher = new Hasher();
        public T2 this[int key]
        {
            get => FindFromIndex(key);
            //set => dic[key ]=new KeyValuePair<T1, T2> () = value;
        }
        public virtual bool IsEmpty()
        { return dic.Count == 0; }

        //! FindFromIndex
        public T2 FindFromIndex(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(), "NCollection_IndexedDataMap::FindFromIndex");
            return dic[theIndex - 1].Value;
            //IndexedDataMapNode* aNode = (IndexedDataMapNode*)myData2[theIndex - 1];
            //return aNode->Value();
        }

        public TheKeyType FindKey(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(), "NCollection_IndexedDataMap::FindKey");
            var aNode = dic[theIndex - 1];
            return aNode.Key;
            //return dic[theIndex].Key;
        }
        //! Returns the Index of already bound Key or appends new Key with specified Item value.
        //! @param theKey1 Key to search (and to bind, if it was not bound already)
        //! @param theItem Item value to set for newly bound Key; ignored if Key was already bound
        //! @return index of Key
        public int Add(TheKeyType theKey1, T2 theItem)
        {
            for (int i = 0; i < dic.Count; i++)
            {
                if (hasher.Equals(dic[i].Key, theKey1))
                    //if (dic[i].Key.Equals(theKey1))
                    return i + 1;
            }
            dic.Add(new KeyValuePair<TheKeyType, T2>(theKey1, theItem));
            return dic.Count;
            //if (Resizable())
            //{
            //    ReSize(Extent());
            //}

            //const Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* pNode = (IndexedDataMapNode*)myData1[iK1];
            //while (pNode)
            //{
            //    if (Hasher::IsEqual(pNode->Key1(), theKey1))
            //    {
            //        return pNode->Index();
            //    }
            //    pNode = (IndexedDataMapNode*)pNode->Next();
            //}

            //const Standard_Integer aNewIndex = Increment();
            //pNode = new(this->myAllocator) IndexedDataMapNode(theKey1, aNewIndex, theItem, myData1[iK1]);
            //myData1[iK1] = pNode;
            //myData2[aNewIndex - 1] = pNode;
            //return aNewIndex;
        }
        //! Substitute
        public void Substitute(int theIndex,
                     TheKeyType theKey1,
                     T2 theItem)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(),
                                          "NCollection_IndexedDataMap::Substitute : " +
                                          "Index is out of range");

            dic[theIndex - 1] = new KeyValuePair<TheKeyType, T2>(theKey1, theItem);
            // check if theKey1 is not already in the map
            //const Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* p = (IndexedDataMapNode*)myData1[iK1];
            //while (p)
            //{
            //    if (Hasher::IsEqual(p->Key1(), theKey1))
            //    {
            //        if (p->Index() != theIndex)
            //        {
            //            throw new Standard_DomainError("NCollection_IndexedDataMap::Substitute : "+
            //                                        "Attempt to substitute existing key");
            //        }
            //        p->Key1() = theKey1;
            //        p->ChangeValue() = theItem;
            //        return;
            //    }
            //    p = (IndexedDataMapNode*)p->Next();
            //}

            //// Find the node for the index I
            //p = (IndexedDataMapNode*)myData2[theIndex - 1];

            //// remove the old key
            //const Standard_Integer iK = Hasher::HashCode(p->Key1(), NbBuckets());
            //IndexedDataMapNode* q = (IndexedDataMapNode*)myData1[iK];
            //if (q == p)
            //    myData1[iK] = (IndexedDataMapNode*)p->Next();
            //else
            //{
            //    while (q->Next() != p)
            //        q = (IndexedDataMapNode*)q->Next();
            //    q->Next() = p->Next();
            //}

            //// update the node
            //p->Key1() = theKey1;
            //p->ChangeValue() = theItem;
            //p->Next() = myData1[iK1];
            //myData1[iK1] = p;
        }

        //! FindIndex
        public int FindIndex(TheKeyType theKey1)
        {
            if (IsEmpty())
                return 0;

            for (int i = 0; i < dic.Count; i++)
            {
                /*  if (hasher != null)
                  {
                      if (hasher.Equals(dic[i].Key, theKey1))
                          return i + 1;
                  }
                  else*/
                {

                    if (dic[i].Key.Equals(theKey1))
                        return i + 1;
                }
            }
            //IndexedDataMapNode* pNode1 = (IndexedDataMapNode*)myData1[Hasher::HashCode(theKey1, NbBuckets())];
            //while (pNode1)
            //{
            //    if (Hasher::IsEqual(pNode1->Key1(), theKey1))
            //    {
            //        return pNode1->Index();
            //    }
            //    pNode1 = (IndexedDataMapNode*)pNode1->Next();
            //}
            return 0;
        }

        public int Extent()
        {
            return dic.Count;

        }
    }


    //! Enumeration covering all execution statuses supported by the class
    //! Message_ExecStatus: 32 statuses per each of 4 types (DONE, WARN, ALARM, FAIL)

    public enum Message_Status
    {
        //! Empty status
        Message_None = 0,

        //! Something done, 32 variants
        Message_Done1 = Message_StatusType.Message_DONE,
        Message_Done2, Message_Done3, Message_Done4, Message_Done5,
        Message_Done6, Message_Done7, Message_Done8, Message_Done9,
        Message_Done10, Message_Done11, Message_Done12, Message_Done13,
        Message_Done14, Message_Done15, Message_Done16, Message_Done17,
        Message_Done18, Message_Done19, Message_Done20, Message_Done21,
        Message_Done22, Message_Done23, Message_Done24, Message_Done25,
        Message_Done26, Message_Done27, Message_Done28, Message_Done29,
        Message_Done30, Message_Done31, Message_Done32,

        //! Warning for possible problem encountered, 32 variants
        Message_Warn1 = Message_StatusType.Message_WARN,
        Message_Warn2, Message_Warn3, Message_Warn4, Message_Warn5,
        Message_Warn6, Message_Warn7, Message_Warn8, Message_Warn9,
        Message_Warn10, Message_Warn11, Message_Warn12, Message_Warn13,
        Message_Warn14, Message_Warn15, Message_Warn16, Message_Warn17,
        Message_Warn18, Message_Warn19, Message_Warn20, Message_Warn21,
        Message_Warn22, Message_Warn23, Message_Warn24, Message_Warn25,
        Message_Warn26, Message_Warn27, Message_Warn28, Message_Warn29,
        Message_Warn30, Message_Warn31, Message_Warn32,

        //! Alarm (severe warning) for problem encountered, 32 variants
        Message_Alarm1 = Message_StatusType.Message_ALARM,
        Message_Alarm2, Message_Alarm3, Message_Alarm4, Message_Alarm5,
        Message_Alarm6, Message_Alarm7, Message_Alarm8, Message_Alarm9,
        Message_Alarm10, Message_Alarm11, Message_Alarm12, Message_Alarm13,
        Message_Alarm14, Message_Alarm15, Message_Alarm16, Message_Alarm17,
        Message_Alarm18, Message_Alarm19, Message_Alarm20, Message_Alarm21,
        Message_Alarm22, Message_Alarm23, Message_Alarm24, Message_Alarm25,
        Message_Alarm26, Message_Alarm27, Message_Alarm28, Message_Alarm29,
        Message_Alarm30, Message_Alarm31, Message_Alarm32,

        //! Execution failed, 32 variants
        Message_Fail1 = Message_StatusType.Message_FAIL,
        Message_Fail2, Message_Fail3, Message_Fail4, Message_Fail5,
        Message_Fail6, Message_Fail7, Message_Fail8, Message_Fail9,
        Message_Fail10, Message_Fail11, Message_Fail12, Message_Fail13,
        Message_Fail14, Message_Fail15, Message_Fail16, Message_Fail17,
        Message_Fail18, Message_Fail19, Message_Fail20, Message_Fail21,
        Message_Fail22, Message_Fail23, Message_Fail24, Message_Fail25,
        Message_Fail26, Message_Fail27, Message_Fail28, Message_Fail29,
        Message_Fail30, Message_Fail31, Message_Fail32
    }

    /**
	 * Tiny class for extended handling of error / execution
	 * status of algorithm in universal way.
	 *
	 * It is in fact a set of integers represented as a collection of bit flags
	 * for each of four types of status; each status flag has its own symbolic 
	 * name and can be set/tested individually.
	 *
	 * The flags are grouped in semantic groups: 
	 * - No flags means nothing done
	 * - Done flags correspond to some operation successfully completed
	 * - Warning flags correspond to warning messages on some 
	 *   potentially wrong situation, not harming algorithm execution
	 * - Alarm flags correspond to more severe warnings about incorrect
	 *   user data, while not breaking algorithm execution
	 * - Fail flags correspond to cases when algorithm failed to complete
	 */
    public class Message_ExecStatus
    {
        //! Definition of types of execution status supported by
        //! the class Message_ExecStatus

        public enum Message_StatusType
        {
            Message_DONE = 0x00000100,
            Message_WARN = 0x00000200,
            Message_ALARM = 0x00000400,
            Message_FAIL = 0x00000800
        }
        //! Check status for being set
        public bool IsSet(Message_Status theStatus)
        {
            switch (TypeOfStatus(theStatus))
            {
                case Message_StatusType.Message_DONE: return (myDone & getBitFlag(theStatus)) != 0;
                case Message_StatusType.Message_WARN: return (myWarn & getBitFlag(theStatus)) != 0;
                case Message_StatusType.Message_ALARM: return (myAlarm & getBitFlag(theStatus)) != 0;
                case Message_StatusType.Message_FAIL: return (myFail & getBitFlag(theStatus)) != 0;
            }
            return false;
        }

        //! Clear all statuses
        public void Clear()
        {
            myDone = myWarn = myAlarm = myFail = (int)Message_Status.Message_None;
        }

        //! Clear one status
        public void Clear(Message_Status theStatus)
        {
            switch (TypeOfStatus(theStatus))
            {
                case Message_StatusType.Message_DONE: myDone &= ~(getBitFlag(theStatus)); return;
                case Message_StatusType.Message_WARN: myWarn &= ~(getBitFlag(theStatus)); return;
                case Message_StatusType.Message_ALARM: myAlarm &= ~(getBitFlag(theStatus)); return;
                case Message_StatusType.Message_FAIL: myFail &= ~(getBitFlag(theStatus)); return;
            }
        }

        //! Returns status type (DONE, WARN, ALARM, or FAIL) 
        static Message_StatusType TypeOfStatus(Message_Status theStatus)
        {
            return (Message_StatusType)((uint)theStatus & (uint)StatusMask.MType);
        }

        static int getBitFlag(int theStatus)
        {
            return 0x1 << (theStatus & (int)StatusMask.MIndex);
        }

        static int getBitFlag(Message_Status theStatus)
        {
            return getBitFlag((int)theStatus);
        }

        int myDone;
        int myWarn;
        int myAlarm;
        int myFail;

        //! Create empty execution status
        public Message_ExecStatus()

        {
            myDone = (int)Message_Status.Message_None;
            myWarn = (int)Message_Status.Message_None;
            myAlarm = (int)Message_Status.Message_None;
            myFail = (int)Message_Status.Message_None;
        }

        //! Sets a status flag
        public void Set(Message_Status theStatus)
        {
            switch (TypeOfStatus(theStatus))
            {
                case Message_StatusType.Message_DONE: myDone |= (getBitFlag((int)theStatus)); break;
                case Message_StatusType.Message_WARN: myWarn |= (getBitFlag((int)theStatus)); break;
                case Message_StatusType.Message_ALARM: myAlarm |= (getBitFlag((int)theStatus)); break;
                case Message_StatusType.Message_FAIL: myFail |= (getBitFlag((int)theStatus)); break;
            }
        }

        //! Mask to separate bits indicating status type and index within the type  
        enum StatusMask
        {
            MType = 0x0000ff00,
            MIndex = 0x000000ff
        };


    }
    //! Definition of types of execution status supported by
    //! the class Message_ExecStatus

    enum Message_StatusType
    {
        Message_DONE = 0x00000100,
        Message_WARN = 0x00000200,
        Message_ALARM = 0x00000400,
        Message_FAIL = 0x00000800
    }
}