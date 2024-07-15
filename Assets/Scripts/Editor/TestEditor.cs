using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class TestEditor : Editor
{

    Material mat;
    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
    }
    private void OnDisable()
    {
        DestroyImmediate(mat);
    }


    public override void OnInspectorGUI()
    {
        PlayerMovement playerMovement = (PlayerMovement)target;

        // Calls normal inspector
        base.OnInspectorGUI();

        TLP.Editor.EditorGraph graph_velocity = new TLP.Editor.EditorGraph(-1, -1, 1, 1, "Velocity", 100);
        // Add a horizontal line with different color
        graph_velocity.AddLineY(0, Color.white);
        graph_velocity.AddLineX(0, Color.white);
        if (playerMovement.motionFunction != null) 
        {
            graph_velocity.AddFunction(x => playerMovement.motionFunction.Velocity(x), Color.blue);
        }
        graph_velocity.Draw();


        TLP.Editor.EditorGraph graph_acceleration = new TLP.Editor.EditorGraph(-1, -10, 1, 10, "Acceleration", 100);
        graph_acceleration.AddLineY(0, Color.white);
        graph_acceleration.AddLineX(0, Color.white);
        if (playerMovement.motionFunction != null)
        {
            graph_acceleration.AddFunction(x => playerMovement.motionFunction.Acceleration(x), Color.blue);
        }
        graph_acceleration.Draw();


        // Print Current Velocity

        // Print Current Acceleration
    }
}
