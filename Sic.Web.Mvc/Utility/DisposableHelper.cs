﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rp3.Web.Mvc.Utility
{
    public class DisposableHelper : IDisposable
    {
        private Action end;

        // When the object is created, write "begin" function
        public DisposableHelper(Action begin, Action end)
        {
            this.end = end;
            begin();
        }

        // When the object is disposed (end of using block), write "end" function
        public void Dispose()
        {
            end();
        }
    }
}
