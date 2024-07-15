using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class TestEditor : Editor
{
    private void OnEnable()
    {
        
    }


    public override void OnInspectorGUI()
    {
        // Calls normal inspector
        base.OnInspectorGUI();

        TLP.Editor.EditorGraph graph = new TLP.Editor.EditorGraph(0, -1, 10, 1, "Just a sin wave", 100);
        graph.AddFunction(x => Mathf.Sin(x));
        graph.Draw();
    }
}
