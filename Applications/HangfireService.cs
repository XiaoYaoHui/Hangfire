using System;

namespace Core.Api.Applications
{
    public class HangfireService
    {
        public void Test(string param)
        {
            Console.WriteLine($"HangfireService： {param}");
        }
    }
}
