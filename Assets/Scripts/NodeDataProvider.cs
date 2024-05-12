
public static class NodeDataProvider
{
    public static Node CenterNode { get; private set; } = new CenterNodes();
    public static Node UpNode { get; private set; } = new UpNodes();
    public static Node DownNode { get; private set; } = new DownNodes();
    public static Node LeftNode { get; private set; } = new LeftNodes();
    public static Node RightNode { get; private set; } = new RightNodes();
    public static Node UpDownNode { get; private set; } = new UpDownNodes();
    public static Node UpRightNode { get; private set; } = new UpRightNodes();
    public static Node UpLeftNode { get; private set; } = new UpLeftNodes();
    public static Node DownLeftNode { get; private set; } = new DownLeftNodes();
    public static Node DownRightNode { get; private set; } = new DownRightNodes();
    public static Node RightLeftNode { get; private set; } = new RightLeftNodes();
    public static Node UpCloseNode { get; private set; } = new UpCloseNodes();
    public static  Node DownCloseNode { get; private set; } = new DownCloseNodes();
    public static Node RightCloseNode { get; private set; } = new RightCloseNodes();
    public static Node LeftCloseNode { get; private set; } = new LeftCloseNodes();
}
