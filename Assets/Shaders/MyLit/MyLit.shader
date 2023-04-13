Shader "Unlit/MyLit"
{
    // Properties set per material, exposed in the material inspector
    Properties
    {
        [Header(Surface options)] // Creates a text header

        // Format the properties like so: <_PropertyName>(<Material Inspector Name>, <DataType>) <DefaultVal>
        // Convention states that property names start with an underscore
        _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _ColorMap("Color", 2D)    = "white" {}
    }
    // SubShaaders allow for different behavior and optionf for different pipelines and platforms
    SubShader
    {
        // These tags are shared by all passes in this sub shader
        // This SubShader is tagged for use in the UniversalPipeline
        Tags{"RenderPipeline" = "UniversalPipeline"}

        // Shaders can have several passes which are used to render different data about the material
        // Each pass has it's own vertex and fragment function and shader variant keywords
        Pass
        {
            Name "ForwardLit" // For debugging
            Tags{"LightMode" = "UniversalForward"} // Pass specific tags. UniversalForward is the value for a color pass
    
            HLSLPROGRAM // Begin HLSL code
            // Register our programmable stage functions
            #pragma vertex Vertex     // Register my custom vertex shader function
            #pragma fragment Fragment // Register my custom fragment shader function

            // Inlcude my custom HLSL code
            #include "MyLitForwardLitPass.hlsl"
            ENDHLSL
        }
    }
}
