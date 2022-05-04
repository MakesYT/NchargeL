namespace NCLCore;

public class Lib
{
    public string path { get; set; }
    public string url { get; set; }
    public string sha1 { get; set; }

    public string name { get; set; }

    // public string verDir { get; set; }
    public bool native { get; set; } = false;
}