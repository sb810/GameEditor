using System.Collections.Generic;
using Plugins.Asset_Cleaner.com.leopotam.ecs.src;
using Plugins.Asset_Cleaner.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Asset_Cleaner.Data {
    class SearchArg : IEcsAutoReset {
        public Object Target;
        public Object Main;
        public string FilePath;
        public Option<Object[]> SubAssets;
        public Scene Scene;
        public List<string> UnusedAssetsFiltered;
        public List<string> UnusedScenesFiltered;

        public void Reset() {
            UnusedAssetsFiltered = default;
            UnusedScenesFiltered = default;
            Target = default;
            Main = default;
            SubAssets = default;
            Scene = default;
            FilePath = default;
        }
    }
}