public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public Node Next { get; set; }

    public Node(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.Next = null;
    }
}