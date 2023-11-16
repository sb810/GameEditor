
using System.Runtime.InteropServices;

namespace Plugins.WebGL
{
    public static class JSPlugin
    {
        [DllImport("__Internal")]
        public static extern string FindGroup();

    }
}
