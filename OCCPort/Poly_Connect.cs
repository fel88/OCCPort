namespace OCCPort
{
    //! Provides an algorithm to explore, inside a triangulation, the
    //! adjacency data for a node or a triangle.
    //! Adjacency data for a node consists of triangles which
    //! contain the node.
    //! Adjacency data for a triangle consists of:
    //! -   the 3 adjacent triangles which share an edge of the triangle,
    //! -   and the 3 nodes which are the other nodes of these adjacent triangles.
    //! Example
    //! Inside a triangulation, a triangle T
    //! has nodes n1, n2 and n3.
    //! It has adjacent triangles AT1, AT2 and AT3 where:
    //! - AT1 shares the nodes n2 and n3,
    //! - AT2 shares the nodes n3 and n1,
    //! - AT3 shares the nodes n1 and n2.
    //! It has adjacent nodes an1, an2 and an3 where:
    //! - an1 is the third node of AT1,
    //! - an2 is the third node of AT2,
    //! - an3 is the third node of AT3.
    //! So triangle AT1 is composed of nodes n2, n3 and an1.
    //! There are two ways of using this algorithm.
    //! -   From a given node you can look for one triangle that
    //! passes through the node, then look for the triangles
    //! adjacent to this triangle, then the adjacent nodes. You
    //! can thus explore the triangulation step by step (functions
    //! Triangle, Triangles and Nodes).
    //! -   From a given node you can look for all the triangles
    //! that pass through the node (iteration method, using the
    //! functions Initialize, More, Next and Value).
    //! A Connect object can be seen as a tool which analyzes a
    //! triangulation and translates it into a series of triangles. By
    //! doing this, it provides an interface with other tools and
    //! applications working on basic triangles, and which do not
    //! work directly with a Poly_Triangulation.
    public class Poly_Connect
    {
    }
}