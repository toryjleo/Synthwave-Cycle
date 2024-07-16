using UnityEditor;
using UnityEngine;
using static TLP.Editor.EditorGraph;

namespace CustomInspector
{
    /// <summary>
    /// Adds custom graphs to track velocity and acceleration for PlayerMovement
    /// </summary>
    [CustomEditor(typeof(global::PlayerMovement))]
    public class PlayerMovement : Editor
    {

        public override void OnInspectorGUI()
        {
            global::PlayerMovement playerMovement = (global::PlayerMovement)target;

            // Calls normal inspector
            base.OnInspectorGUI();
            if (playerMovement.motionFunction != null)
            {
                float t = playerMovement.GetX;


                GraphMovement(t, -1, 1, x => playerMovement.motionFunction.Velocity(x), "Velocity");
                EditorGUILayout.LabelField("Calculated velocity: " + playerMovement.motionFunction.Velocity(t));
                EditorGUILayout.LabelField("Current velocity: " + (playerMovement.Velocity / playerMovement.yScale).ToString("F3"));
                EditorGUILayout.LabelField("Current velocity, Scaled: " + (playerMovement.yScale * playerMovement.Velocity).ToString("F3"));
                EditorGUILayout.LabelField("Current t: " + t);


                GraphMovement(t, -1, 1.1f, x => playerMovement.motionFunction.Acceleration(x), "Acceleration");
                EditorGUILayout.LabelField("Calculated Acceleration: " + playerMovement.motionFunction.Acceleration(t));
                EditorGUILayout.LabelField("Current Acceleration: " + playerMovement.currentAcceleration.ToString("F3"));
                EditorGUILayout.LabelField("Current Acceleration, Scaled: " + (playerMovement.yScale * playerMovement.currentAcceleration).ToString("F3"));
                
            }

            
        }

        /// <summary>
        /// Graphs the movement of the given function in the inspector
        /// </summary>
        /// <param name="t">The horizontal variable to sample</param>
        /// <param name="minY">Minimum y to graph to</param>
        /// <param name="maxY">Maximum y to graph to</param>
        /// <param name="func">Function to plot</param>
        /// <param name="name">Name to be displayed in inspector</param>
        public void GraphMovement(float t, float minY, float maxY, GraphFunction func, string name)
        {

            TLP.Editor.EditorGraph graph_velocity = new TLP.Editor.EditorGraph(-1, minY, 1, maxY, name, 100);
            // Add a horizontal line with different color
            graph_velocity.AddLineY(0, Color.white);
            graph_velocity.AddLineX(0, Color.white);

            graph_velocity.AddFunction(func, Color.blue);

            graph_velocity.AddLineX(t, Color.red);
            graph_velocity.AddLineY(func(t), Color.red);

            graph_velocity.Draw();

        }
    }
}

