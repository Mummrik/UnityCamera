using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(InputManager))]
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

        // If no player is set in the inspector try to get one from the scene instead
        if (player == null)
        {
            player = GameObject.Find("Player");
            Assert.IsNotNull(player, "Couldn't find a player GameObject! Please add one to the script or scene (Name it Player).");
        }
        inputManager = gameObject.GetComponent<InputManager>();
    }
}
