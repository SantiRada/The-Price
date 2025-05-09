using UnityEngine;

public class DestroyShield : MonoBehaviour {

    private Room _roomInScene;

    private void Start()
    {
        _roomInScene = FindAnyObjectByType<Room>();

        _roomInScene.SetShieldToNull();
    }
}
