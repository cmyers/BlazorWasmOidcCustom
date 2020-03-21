using BlazorWebAssemblyTest.Shared.Interfaces;

namespace BlazorWebAssemblyTest.Shared.Services
{
    public class CounterService : ICounterService
    {
        private int _count;
        public int GetCount()
        {
            return _count;
        }

        public void Increment()
        {
            _count++;
        }
    }
}
