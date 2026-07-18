namespace TKBO
{
    //! The history keeps the following relations between the input shapes
    //! (S1, ..., Sm) and output shapes (T1, ..., Tn):
    //! 1) an output shape Tj is generated from an input shape Si: Tj <= G(Si);
    //! 2) a output shape Tj is modified from an input shape Si: Tj <= M(Si);
    //! 3) an input shape (Si) is removed: R(Si) == 1.
    //!
    //! The relations are kept only for shapes of types vertex, edge, face, and
    //! solid.
    //!
    //! The last relation means that:
    //! 1) shape Si is not an output shape and
    //! 2) no any shape is modified (produced) from shape Si:
    //! R(Si) == 1 ==> Si != Tj, M(Si) == 0.
    //!
    //! It means that the input shape cannot be removed and modified
    //! simultaneously. However, the shapes may be generated from the
    //! removed shape. For instance, in Fillet operation the edges
    //! generate faces and then are removed.
    //!
    //! No any shape could be generated and modified from the same shape
    //! simultaneously: sets G(Si) and M(Si) are not intersected
    //! (G(Si) ^ M(Si) == 0).
    //!
    //! Each output shape should be:
    //! 1) an input shape or
    //! 2) generated or modified from an input shape (even generated from the
    //!   implicit null shape if necessary):
    //!   Tj == Si V (exists Si that Tj <= G(Si) U M(Si)).
    //!
    //! Recommendations to choose between relations 'generated' and 'modified':
    //! 1) a shape is generated from input shapes if it dimension is greater or
    //!   smaller than the dimensions of the input shapes;
    //! 2) a shape is generated from input shapes if these shapes are also output
    //!   shapes;
    //! 3) a shape is generated from input shapes of the same dimension if it is
    //!   produced by joining shapes generated from these shapes;
    //! 4) a shape is modified from an input shape if it replaces the input shape by
    //!   changes of the location, the tolerance, the bounds of the parametric
    //!   space (the faces for a solid), the parametrization and/or by applying of
    //!   an approximation;
    //! 5) a shape is modified from input shapes of the same dimension if it is
    //!   produced by joining shapes modified from these shapes.
    //!
    //! Two sequential histories:
    //! - one history (H12) of shapes S1, ..., Sm to shapes T1, ..., Tn and
    //! - another history (H23) of shapes T1, ..., Tn to shapes Q1, ..., Ql
    //! could be merged to the single history (H13) of shapes S1, ..., Sm to shapes
    //! Q1, ..., Ql.
    //!
    //! During the merge:
    //! 1) if shape Tj is generated from shape Si then each shape generated or
    //!   modified from shape Tj is considered as a shape generated from shape Si
    //!   among shapes Q1, ..., Ql:
    //!   Tj <= G12(Si), Qk <= G23(Tj) U M23(Tj) ==> Qk <= G13(Si).
    //! 2) if shape Tj is modified from shape Si, shape Qk is generated from shape
    //!   Tj then shape Qk is considered as a shape generated from shape Si among
    //!   shapes Q1, ..., Ql:
    //!   Tj <= M12(Si), Qk <= G23(Tj) ==> Qk <= G13(Si);
    //! 3) if shape Tj is modified from shape Si, shape Qk is modified from shape
    //!   Tj then shape Qk is considered as a shape modified from shape Si among
    //!   shapes Q1, ..., Ql:
    //!   Tj <= M12(Si), Qk <= M23(Tj) ==> Qk <= M13(Si);
    public class BRepTools_History 
    {
    }
}