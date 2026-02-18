using UnityEngine;

[CreateAssetMenu(fileName = "MechanicConfig", menuName = "Config/MechanicConfig")]
public class MechanicConfig : ScriptableObject
{
    public bool enableJump;
    public bool enableSwipe;
    public float speed;
}
