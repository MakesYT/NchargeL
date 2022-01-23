namespace NCLCore
{
    public class Client
    {   
        /// <summary>
        /// 我的世界客户端名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 我的世界版本
        /// </summary>
        public string McVer=null;
        /// <summary>
        /// 是否为Ncharge客户端
        /// </summary>
        public bool Ncharge;
        /// <summary>
        /// Ncharge客户端版本
        /// </summary>
        public string NchargeVer;
        public int Id { get; set; }
        public Client()
        {
           
        }
        public bool isNotNull()
        {
            if (Name != null && McVer != null)
            {
                return true;
            }else return false;
        }
    }
}
