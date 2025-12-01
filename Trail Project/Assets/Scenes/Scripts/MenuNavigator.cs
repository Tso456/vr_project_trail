using UnityEngine;
using System.Collections.Generic;

public class MenuNavigator : MonoBehaviour
{
    [Header("Menu Panels")]
    [Tooltip("Add all your menu panels here in any order")]
    public List<GameObject> menuPanels = new List<GameObject>();
    
    [Header("Settings")]
    [SerializeField] private int startingMenuIndex = 0;
    [SerializeField] private bool hideOthersOnStart = true;
    
    private int currentMenuIndex = -1;
    private GameObject currentActiveMenu;
    
    // Stack for back navigation
    private Stack<int> menuHistory = new Stack<int>();
    
    void Start()
    {
        // Validate menu panels
        if (menuPanels == null || menuPanels.Count == 0)
        {
            Debug.LogError("MenuNavigator: No menu panels assigned!");
            return;
        }
        
        // Hide all menus initially if specified
        if (hideOthersOnStart)
        {
            HideAllMenus();
        }
        
        // Show starting menu
        ShowMenu(startingMenuIndex);
    }
    
    #region Public Navigation Methods
    
    /// <summary>
    /// Shows a specific menu by index
    /// </summary>
    public void ShowMenu(int menuIndex)
    {
        if (!IsValidIndex(menuIndex)) return;
        
        // Save current menu to history (if navigating to a different menu)
        if (currentMenuIndex != -1 && currentMenuIndex != menuIndex)
        {
            menuHistory.Push(currentMenuIndex);
        }
        
        // Hide current menu
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }
        
        // Show new menu
        currentMenuIndex = menuIndex;
        currentActiveMenu = menuPanels[menuIndex];
        currentActiveMenu.SetActive(true);
        
        Debug.Log($"Navigated to menu: {currentActiveMenu.name}");
    }
    
    /// <summary>
    /// Shows a menu by GameObject name
    /// </summary>
    public void ShowMenuByName(string menuName)
    {
        int index = menuPanels.FindIndex(menu => menu.name == menuName);
        
        if (index >= 0)
        {
            ShowMenu(index);
        }
        else
        {
            Debug.LogWarning($"Menu with name '{menuName}' not found!");
        }
    }
    
    /// <summary>
    /// Goes back to the previous menu in history
    /// </summary>
    public void GoBack()
    {
        if (menuHistory.Count > 0)
        {
            int previousMenuIndex = menuHistory.Pop();
            
            // Hide current menu
            if (currentActiveMenu != null)
            {
                currentActiveMenu.SetActive(false);
            }
            
            // Show previous menu (without adding to history)
            currentMenuIndex = previousMenuIndex;
            currentActiveMenu = menuPanels[previousMenuIndex];
            currentActiveMenu.SetActive(true);
            
            Debug.Log($"Went back to menu: {currentActiveMenu.name}");
        }
        else
        {
            Debug.Log("No menu history to go back to");
        }
    }
    
    /// <summary>
    /// Clears the navigation history
    /// </summary>
    public void ClearHistory()
    {
        menuHistory.Clear();
    }
    
    /// <summary>
    /// Toggles a menu on/off without affecting navigation history
    /// </summary>
    public void ToggleMenu(int menuIndex)
    {
        if (!IsValidIndex(menuIndex)) return;
        
        GameObject menu = menuPanels[menuIndex];
        menu.SetActive(!menu.activeSelf);
    }
    
    /// <summary>
    /// Shows multiple menus at once (for overlay menus)
    /// </summary>
    public void ShowAdditionalMenu(int menuIndex)
    {
        if (!IsValidIndex(menuIndex)) return;
        
        menuPanels[menuIndex].SetActive(true);
    }
    
    /// <summary>
    /// Hides a specific menu without changing navigation
    /// </summary>
    public void HideMenu(int menuIndex)
    {
        if (!IsValidIndex(menuIndex)) return;
        
        menuPanels[menuIndex].SetActive(false);
    }
    
    /// <summary>
    /// Hides all menus
    /// </summary>
    public void HideAllMenus()
    {
        foreach (GameObject menu in menuPanels)
        {
            if (menu != null)
            {
                menu.SetActive(false);
            }
        }
        
        currentActiveMenu = null;
        currentMenuIndex = -1;
    }
    
    #endregion
    
    #region Helper Methods
    
    private bool IsValidIndex(int index)
    {
        if (index < 0 || index >= menuPanels.Count)
        {
            Debug.LogWarning($"Menu index {index} is out of range! Valid range: 0-{menuPanels.Count - 1}");
            return false;
        }
        
        if (menuPanels[index] == null)
        {
            Debug.LogWarning($"Menu at index {index} is null!");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets the current active menu index
    /// </summary>
    public int GetCurrentMenuIndex()
    {
        return currentMenuIndex;
    }
    
    /// <summary>
    /// Gets the current active menu GameObject
    /// </summary>
    public GameObject GetCurrentMenu()
    {
        return currentActiveMenu;
    }
    
    #endregion
    
    #region Unity Editor Helpers
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        // Auto-populate menu panels if empty
        if (menuPanels.Count == 0)
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child != transform && child.parent == transform)
                {
                    menuPanels.Add(child.gameObject);
                }
            }
        }
    }
    #endif
    
    #endregion
}
