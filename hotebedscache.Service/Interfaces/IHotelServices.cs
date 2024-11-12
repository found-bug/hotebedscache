using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotebedscache.Service.Interfaces
{
    public interface IHotelServices
    {
        Task<string> callapi();
    }
}
