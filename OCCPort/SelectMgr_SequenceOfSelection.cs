using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class SelectMgr_SequenceOfSelection : List<SelectMgr_Selection>
    {
        public void Append(SelectMgr_Selection aNewSel)
        {
            Add(aNewSel);
        }
    }
}