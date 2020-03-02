using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    InputManager inputManager;
    public GameObject player;

    private void Awake()
    {
        // Create a singleton of this class
        if (instance != null)
        {
            Destroy(instance);
            instance = this;
        }
        else
        {
            instance = this;
        }

        // Get a reference to the player
        player = GameObject.Find("Player");
        inputManager = gameObject.AddComponent<InputManager>();
    }
}
