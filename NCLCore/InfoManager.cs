namespace NCLCore
{

    public class InfoManager
    {
        // public InfoType type{get;set;}
        public Info info = new("1", InfoType.success);

        public event EventHandler<Info>? PropertyChanged;

        public void Info(Info info)
        {
            this.info = info;
            PropertyChanged?.Invoke(this, info);
        }
    }
}
