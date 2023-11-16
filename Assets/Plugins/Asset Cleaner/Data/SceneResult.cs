using Plugins.Asset_Cleaner.com.leopotam.ecs.src;

namespace Plugins.Asset_Cleaner.Data {
    class SceneResult : IEcsAutoReset {
        public string PathNicified;

        public void Reset() {
            PathNicified = default;
        }
    }
}