using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Trovit
{
    public interface ITrovitParser<T>
    {
        public T Parse(string content);
    }
}
