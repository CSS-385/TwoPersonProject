using UnityEditor;
using UnityEngine;

namespace TwoPersonProject
{
    [CustomEditor(typeof(LevelPart))]
    public class LevelPartEditor : Editor
    {
        private void OnSceneGUI()
        {
            LevelPart part = (LevelPart)target;

            Handles.color = Color.green;
            Undo.RecordObject(part, "kek");
            part.leftConnection = Handles.FreeMoveHandle((Vector3)part.leftConnection + part.transform.position, 0.25f, Vector3.one / 2, Handles.DotHandleCap) - part.transform.position;
            part.rightConnection = Handles.FreeMoveHandle((Vector3)part.rightConnection + part.transform.position, 0.25f, Vector3.one / 2, Handles.DotHandleCap) - part.transform.position;

            PrefabUtility.RecordPrefabInstancePropertyModifications(part);
        }
    }
}
