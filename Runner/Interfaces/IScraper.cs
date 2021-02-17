using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runner.Interfaces
{
    public interface IScraper
    {
        Task<List<Advertisement>> RunAsync(string queryUrl);
    }
}
