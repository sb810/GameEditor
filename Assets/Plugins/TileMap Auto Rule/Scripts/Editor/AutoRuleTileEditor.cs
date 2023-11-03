using UnityEditor;
using UnityEngine;

// ----------------------------------------------------------------------------
// Author: Alexandre Brull
// https://brullalex.itch.io/
// ----------------------------------------------------------------------------

namespace Plugins.TileMap_Auto_Rule.Scripts.Editor
{
    [CustomEditor(typeof(TerrainAutoRuleTile))]
    [CanEditMultipleObjects]
    public class ObjectBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TerrainAutoRuleTile myScript = (TerrainAutoRuleTile)target;
            if (GUILayout.Button("Build Rule Tile"))
            {
                myScript.OverrideRuleTile();
            }
        }
    }
}
