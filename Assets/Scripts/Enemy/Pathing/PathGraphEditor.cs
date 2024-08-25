using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(PathGraph))]
public class PathGraphEditor : Editor
{

    private void OnSceneGUI()
    {
        PathGraph path = target as PathGraph;

        for (int i = 0; i < path.positions.Length; i++)
        {
            Transform prev = i > 0 ? path.positions[i - 1] : null;
            if (path.positions[i] != null && path.positions[i] != prev)
            {
                path.positions[i].position = Handles.PositionHandle(path.positions[i].position, Quaternion.identity);
                continue;
            } 
            Transform newPoint = new GameObject($"{path.Target  .name}'s position #{i + 1}").transform;
            newPoint.parent = path.Target;
            path.positions[i] = newPoint;
        }
    }

}

