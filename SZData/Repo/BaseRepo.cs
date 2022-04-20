using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZData.Repo
{
    public class BaseRepo
    {
        public string ConnectionText => (ConfigurationManager.ConnectionStrings["RainbowConnection"] == null) ? "" : ConfigurationManager.ConnectionStrings["RainbowConnection"].ConnectionString;
    }
}
