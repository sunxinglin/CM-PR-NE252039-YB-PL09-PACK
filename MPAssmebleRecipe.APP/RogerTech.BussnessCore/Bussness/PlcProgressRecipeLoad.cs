using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using RogerTech.Tool;

namespace RogerTech.BussnessCore
{
    public class PlcProgressRecipeLoad : PlcInProgressBase
    {

        public PlcProgressRecipeLoad(string name):base(name)
        {

        }
        public ILog Logger { get; }

        public Group PlcGroup { get; set; }
        

        public EventHandler ProgressComplete { get; set; }
      

        public EventHandler ProgressStarted { get; set; }
        
        public void Excute()
        {
            throw new NotImplementedException();
        }
    }
}
