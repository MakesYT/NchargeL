using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NchargeLMaui
{
    public class NumericValidationTriggerAction : TriggerAction<Shell>
    {
        protected override void Invoke(Shell entry)
        {
            entry.FlyoutWidth = 188;
            
        }
    }
}
