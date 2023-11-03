using System.Collections.Generic;
using Plugins.Asset_Cleaner.com.leopotam.ecs.src;
using UnityEditor;
using UnityEngine;

namespace Plugins.Asset_Cleaner.Data {
    class SearchResultGui : IEcsAutoReset {
        public SerializedObject SerializedObject;
        public List<PropertyData> Properties;
        public GUIContent Label;
        public string TransformPath;

        public void Reset() {
            SerializedObject?.Dispose();
            SerializedObject = default;

            if (Properties != default)
                foreach (var propertyData in Properties) {
                    propertyData.Property.Dispose();
                }

            Properties = default;
            Label = default;
            TransformPath = default;
        }

        public class PropertyData {
            public SerializedProperty Property;
            public GUIContent Content;
        }
    }
}