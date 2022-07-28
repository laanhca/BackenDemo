
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance;
    public FloatingJoystick Joystick;
    [SerializeField] private Button btnLeaderboard;
    [SerializeField] private Text txtPoint;

    private void Awake()
    {
        Instance = this;
    }
}
