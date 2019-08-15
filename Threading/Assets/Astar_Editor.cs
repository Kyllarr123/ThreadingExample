using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Russell
{
    [CustomEditor(typeof(AStar))]
    public class Astar_Editor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            AStar aStar = target as AStar;
            //GUILayout
            if (GUILayout.Button("ReRun"))
            {
                aStar.Rerun();
            }

        }
    }


}
