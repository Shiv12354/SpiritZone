using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface IConfigRepo: IDisposable
    {
        string Get(string key);
        IList<ConfigModel> GetAll();
    }
}
