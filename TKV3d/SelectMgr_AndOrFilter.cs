namespace TKV3d
{
    //! A framework to define an OR or AND selection filter.
    //! To use an AND selection filter call SetUseOrFilter with False parameter.
    //! By default the OR selection filter is used.
    public class SelectMgr_AndOrFilter : SelectMgr_CompositionFilter
    {
        public SelectMgr_AndOrFilter(SelectMgr_FilterType selectMgr_FilterType_OR)
        {
        }
        SelectMgr_FilterType myFilterType; //!< selection filter type. SelectMgr_TypeFilter_OR by default.
        Graphic3d_NMapOfTransient myDisabledObjects; //!< disabled objects.
                                                     //!  Selection isn't applied to these objects.
        public override bool IsOk(SelectMgr_EntityOwner theObj)
        {
            SelectMgr_SelectableObject aSelectable = theObj.Selectable();

            if (myDisabledObjects != null && myDisabledObjects.Contains(aSelectable))
            {
                return false;
            }

            for (SelectMgr_ListIteratorOfListOfFilter anIter = new(myFilters); anIter.More(); anIter.Next())
            {
                bool isOK = anIter.Value().IsOk(theObj);
                if (isOK && myFilterType == SelectMgr_FilterType.SelectMgr_FilterType_OR)
                {
                    return true;
                }
                else if (!isOK && myFilterType == SelectMgr_FilterType.SelectMgr_FilterType_AND)
                {
                    return false;
                }
            }

            if (myFilterType == SelectMgr_FilterType.SelectMgr_FilterType_OR && !myFilters.IsEmpty())
            {
                return false;
            }
            return true;
        }

    }
}

