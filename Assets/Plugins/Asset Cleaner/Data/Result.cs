using Plugins.Asset_Cleaner.com.leopotam.ecs.src;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Asset_Cleaner.Data {
    class Result : IEcsAutoReset {
        public string FilePath;
        public Object File;
        public Object MainFile;
        public GameObject RootGo;

        public void Reset() {
            FilePath = default;
            File = default;
            MainFile = default;
            RootGo = default;
        }
    }
}