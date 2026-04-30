namespace OCCPort
{
    // this structure records one of the edges starting from a node
    public class polyedge
    {
        public polyedge next;         // the next edge in the list
        public int[] nt = new int [2]; // the two adjacent triangles
        public int[] nn = new int[2]; // the two adjacent nodes
        public int nd;    // the second node of the edge

    };
}