﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KouXiaGu.Concurrent
{

    public interface IRequestDispatcher
    {
        int Count { get; }
        IRequest Add(IRequest request);
    }
}
