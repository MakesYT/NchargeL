using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{

    internal class InfoManager
    {
        public InfoType type{get;set;}
        private Info _info = new("1", "info");
        public Info info
        {
            get { return _info; }
            set
            {
                _info = value;
                this.OnWorkStateChanged(new EventArgs());
            }
        }
        public event EventHandler PropertyChanged;
        public void OnWorkStateChanged(EventArgs eventArgs)
        {
            if (this.PropertyChanged != null)//判断事件是否有处理函数
            {
                this.PropertyChanged(this, eventArgs);
            }

        }
    }
}
