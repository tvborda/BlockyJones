using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelStudy
{
    [CustomEditor(typeof(Tile), true)]
    [CanEditMultipleObjects]
    public class TileInspector : Editor
    {
        Tile levelTile;

        public void OnEnable()
        {
            levelTile = (Tile)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (Application.isPlaying == false)
            {
                levelTile.InspectorUpdate();
            }
        }
    }
}