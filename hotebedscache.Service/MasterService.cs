using Hotebedscache.Domain.Entities;
using Hotebedscache.Service.Interfaces;

namespace Hotebedscache.Service
{
    public class MasterService : IMasterServices
    {
        private readonly HotebedContext context;
        public MasterService(HotebedContext context)
        {
            this.context = context;
        }
        public List<City> GetCities()
        {
            var response = new List<City>();
            response = context.Cities.Select(s => new City
            {
                Id = s.Id,
                Name = s.Name

            }).ToList();
            return response;
        } 
    }
}
