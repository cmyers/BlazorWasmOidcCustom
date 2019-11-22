using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorClientTest.Shared.Interfaces
{
    public interface ICounterService
    {
        void Increment();
        int GetCount();
    }
}
