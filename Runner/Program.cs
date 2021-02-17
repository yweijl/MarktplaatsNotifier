using System;
using System.Threading.Tasks;

namespace Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var runner = new Runner();
                await runner.RunAsync("url");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
