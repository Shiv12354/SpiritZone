using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZModels;

namespace SZData.Interfaces
{
    public interface IConfigService: IDisposable
    {
        string GetValue(string key);
        IList<ConfigModel> GetAll();
    }
}
