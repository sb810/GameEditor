using Plugins.Asset_Cleaner.Systems.External;
using UnityEngine;

namespace Plugins.Asset_Cleaner.Data.Globals {
    class WindowData {
        public bool ExpandFiles;
        public bool ExpandScenes;
        public Vector2 ScrollPos;
        public CleanerStyleAsset.Style Style;
        public GUIContent SceneFoldout;
        public PrevClick Click;
        public AufWindow Window;
        public FindModeEnum FindFrom;
    }
}