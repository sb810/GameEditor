using Plugins.Asset_Cleaner.com.leopotam.ecs.src;
using UnityEngine.SceneManagement;

namespace Plugins.Asset_Cleaner.Data {
    class SceneDetails : IEcsAutoReset {
        public string Path;
        public Scene Scene;
        public bool SearchRequested;
        public bool SearchDone;
        public bool WasOpened;

        public void Reset() {
            Path = default;
            Scene = default;
            SearchRequested = default;
            SearchDone = default;
            WasOpened = default;
        }
    }
}