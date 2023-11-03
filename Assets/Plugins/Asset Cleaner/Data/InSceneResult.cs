using Plugins.Asset_Cleaner.com.leopotam.ecs.src;

namespace Plugins.Asset_Cleaner.Data {
    class InSceneResult : IEcsAutoReset {
        public string ScenePath;

        public void Reset() {
            ScenePath = default;
        }
    }
}