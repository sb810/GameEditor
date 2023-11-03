using UnityEngine;

namespace Plugins.Asset_Cleaner.Data {
    public struct PrevClick {
        const float DoubleClickTime = 0.5f;
        Object _target;
        float _timeClicked;

        public PrevClick(Object target) {
            _target = target;
            _timeClicked = Time.realtimeSinceStartup;
        }

        public bool IsDoubleClick(Object o) {
            return _target == o && Time.realtimeSinceStartup - _timeClicked < DoubleClickTime;
        }
    }
}