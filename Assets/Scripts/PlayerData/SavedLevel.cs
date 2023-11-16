using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerData
{
    [Serializable]
    public class SavedLevel
    {
        [FormerlySerializedAs("index")] public List<int> indexes = new();
        public List<Vector3> positions = new();
        public List<Quaternion> rotations = new();
        public List<Vector3> scales = new();
        public List<string> prefabs = new();
        public int backgroundIndex;
        public string timestamp = DateTime.Now.ToString("MM/dd - HH:mm");
    }
}