using UnityEngine;

public class InfectedFood : MonoBehaviour
{
    public Camera minimapCamera;
    void Awake() => GetComponentInChildren<Canvas>().worldCamera = minimapCamera;
}
