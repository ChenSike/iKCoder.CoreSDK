using System;
using System.Collections.Generic;
using System.Text;

namespace iKCoderSDK
{
    public interface Basic_InternalPlugin
    {
        object actionGet(object paramsList);
        object actionSet(object paramsList);
    }
}
