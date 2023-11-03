using Plugins.Asset_Cleaner.com.leopotam.ecs.src;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Asset_Cleaner.Data {
    class SelectionChanged : IEcsAutoReset {
        public Object Target;
        public Scene Scene;
        public FindModeEnum From;

        public void Reset() {
            Target = default;
            Scene = default;
            From = default;
        }
    }
}