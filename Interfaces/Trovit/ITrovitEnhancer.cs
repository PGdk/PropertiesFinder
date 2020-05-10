using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Trovit
{
    public interface ITrovitEnhancer
    {
        public Entry Enhance(Entry entry);
    };

}
