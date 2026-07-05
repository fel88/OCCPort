namespace TKService
{
    //! Definition of all available hatch styles.
    public enum Aspect_HatchStyle
    {
        Aspect_HS_SOLID = 0,  // TEL_HS_SOLID (no hatching)
        Aspect_HS_HORIZONTAL = 7,  // TEL_HS_HORIZONTAL
        Aspect_HS_HORIZONTAL_WIDE = 11, // TEL_HS_HORIZONTAL_SPARSE
        Aspect_HS_VERTICAL = 8,  // TEL_HS_VERTICAL
        Aspect_HS_VERTICAL_WIDE = 12, // TEL_HS_VERTICAL_SPARSE
        Aspect_HS_DIAGONAL_45 = 5,  // TEL_HS_DIAG_45
        Aspect_HS_DIAGONAL_45_WIDE = 9,  // TEL_HS_DIAG_45_SPARSE
        Aspect_HS_DIAGONAL_135 = 6,  // TEL_HS_DIAG_135
        Aspect_HS_DIAGONAL_135_WIDE = 10, // TEL_HS_DIAG_135_SPARSE
        Aspect_HS_GRID = 3,  // TEL_HS_GRID
        Aspect_HS_GRID_WIDE = 4,  // TEL_HS_GRID_SPARSE
        Aspect_HS_GRID_DIAGONAL = 1,  // TEL_HS_CROSS
        Aspect_HS_GRID_DIAGONAL_WIDE = 2,  // TEL_HS_CROSS_SPARSE
        Aspect_HS_NB = 13,
    };
}
