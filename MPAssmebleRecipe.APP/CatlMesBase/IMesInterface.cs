using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatlMesBase
{
    public interface IMesInterface
    {
        List<Object> GetResult(List<Object> objs);
        List<Object> GetResultSimulation(List<Object> objs);
    }
}
