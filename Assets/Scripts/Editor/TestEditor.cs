using UnityEditor;
using UnityEngine;
using static TLP.Editor.EditorGraph;

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
        if (playerMovement.motionFunction != null)
        {
            float t = playerMovement.Gett;


            Foo(t, -1, 1, x => playerMovement.motionFunction.Velocity(x), "Velocity");

            Foo(t, -10, 10, x => playerMovement.motionFunction.Acceleration(x), "Acceleration");
        }

        // Print Current Velocity

        // Print Current Acceleration
    }

    public void Foo(float t, float minY, float maxY, GraphFunction func, string name) 
    {

        TLP.Editor.EditorGraph graph_velocity = new TLP.Editor.EditorGraph(-1, minY, 1, maxY, name, 100);
        // Add a horizontal line with different color
        graph_velocity.AddLineY(0, Color.white);
        graph_velocity.AddLineX(0, Color.white);

        graph_velocity.AddFunction(func, Color.blue);

        graph_velocity.AddLineX(t, Color.red);

        graph_velocity.Draw();

    }
}


