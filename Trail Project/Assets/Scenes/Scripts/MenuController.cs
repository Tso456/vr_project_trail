using UnityEngine;

public class MenuController : MonoBehaviour
{

    private MenuNavigator menuNav;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuNav = FindFirstObjectByType<MenuNavigator>();
    }

    public void OpenHelp()
    {
        menuNav.ShowMenuByName("Help Menu");
    }

    public void OpenPOISelection()
    {
        menuNav.ShowMenuByName("POI Selection Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
