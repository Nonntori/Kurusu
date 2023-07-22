using UnityEngine;

[CreateAssetMenu (fileName = "Trail Config", menuName = "Weapon/Weapon Trail Config", order = 4)]
public class TrailConfig : ScriptableObject
{
    public Material Material;
    public AnimationCurve WidthCurve;
    public Gradient Color;
    
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;

    public float MissDistance = 1.0f;
    public float SimulationSpeed = 1.0f;

}
