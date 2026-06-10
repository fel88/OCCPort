namespace TKXSBASE
{
    //! This class defines a list of Entities (Transient Objects),
    //! it can be used as a field of other Transient classes, with
    //! these features :
    //! - oriented to define a little list, that is, slower than an
    //! Array or a Map of Entities for a big count (about 100 and
    //! over), but faster than a Sequence
    //! - allows to work as a Sequence, limited to Clear, Append,
    //! Remove, Access to an Item identified by its rank in the list
    //! - space saving, compared to a Sequence, especially for little
    //! amounts; better than an Array for a very little amount (less
    //! than 10) but less good for a greater amount
    //!
    //! Works in conjunction with EntityCluster
    //! An EntityList gives access to a list of Entity Clusters, which
    //! are chained (in one sense : Single List)
    //! Remark : a new Item may not be Null, because this is the
    //! criterium used for "End of List"
    public class Interface_EntityList
    {

    }
}
