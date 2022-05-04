namespace NCLCore;

public class Client
{
    public string dir;

    /// <summary>
    ///     我的世界版本
    /// </summary>
    public string McVer = null;

    /// <summary>
    ///     是否为Ncharge客户端
    /// </summary>
    public bool Ncharge;

    /// <summary>
    ///     Ncharge客户端版本
    /// </summary>
    public string NchargeVer;

    public string assets { get; set; }
    public bool Forge { get; set; }

    /// <summary>
    ///     .minecraft文件夹目录
    /// </summary>
    public string rootdir { get; set; }

    /// <summary>
    ///     我的世界客户端名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     ID排序
    /// </summary>
    public int Id { get; set; }

    public bool isNotNull()
    {
        if (Name != null && McVer != null && dir != null)
            return true;
        return false;
    }
}