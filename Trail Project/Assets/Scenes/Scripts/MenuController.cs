using UnityEngine;

public class MenuController : MonoBehaviour
{

    [SerializeField] private MenuNavigator menuNav;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menuNav == null)
        {
            menuNav = FindFirstObjectByType<MenuNavigator>();
        }

        if (menuNav == null)
        {
            Debug.LogError("MenuController: No MenuNavigator found in scene.");
        }
    }

    public void OpenHelp()
    {
        if (menuNav == null) return;
        menuNav.ShowMenuByName("Help Menu");
    }

    public void OpenPOISelection()
    {
        if (menuNav == null) return;
        menuNav.ShowMenuByName("POI Selection Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
