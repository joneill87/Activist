using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Data
{
    public interface IDataParserFactory
    {
        IDataParser Create(DataFormats outputFormat);
    }
}
