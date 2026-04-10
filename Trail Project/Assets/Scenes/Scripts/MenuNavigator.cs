using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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

        GameObject targetMenu = menuPanels[menuIndex];

        // Ignore duplicate request to avoid unnecessary state churn.
        if (currentActiveMenu == targetMenu)
        {
            return;
        }
        
        // Save current menu to history (if navigating to a different menu)
        if (currentMenuIndex != -1)
        {
            menuHistory.Push(currentMenuIndex);
        }
        
        // Hide current menu unless it is a parent of the menu we are about to show.
        if (currentActiveMenu != null && !targetMenu.transform.IsChildOf(currentActiveMenu.transform))
        {
            currentActiveMenu.SetActive(false);
        }

        // Ensure the target and its parent chain are active so nested menus can become visible.
        ActivateHierarchy(targetMenu.transform);
        
        // Show new menu
        currentMenuIndex = menuIndex;
        currentActiveMenu = targetMenu;
        currentActiveMenu.SetActive(true);

        // Keep newly shown menu on top of sibling menus when using a shared canvas hierarchy.
        currentActiveMenu.transform.SetAsLastSibling();

        LogVisibilityState(currentActiveMenu);
        
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
            if (!IsValidIndex(previousMenuIndex))
            {
                return;
            }

            GameObject previousMenu = menuPanels[previousMenuIndex];
            
            // Hide current menu
            if (currentActiveMenu != null && !previousMenu.transform.IsChildOf(currentActiveMenu.transform))
            {
                currentActiveMenu.SetActive(false);
            }

            ActivateHierarchy(previousMenu.transform);
            
            // Show previous menu (without adding to history)
            currentMenuIndex = previousMenuIndex;
            currentActiveMenu = previousMenu;
            currentActiveMenu.SetActive(true);
            currentActiveMenu.transform.SetAsLastSibling();

            LogVisibilityState(currentActiveMenu);
            
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

    private void ActivateHierarchy(Transform target)
    {
        Transform cursor = target;
        while (cursor != null)
        {
            if (!cursor.gameObject.activeSelf)
            {
                cursor.gameObject.SetActive(true);
            }

            if (cursor == transform)
            {
                break;
            }

            cursor = cursor.parent;
        }
    }

    private void LogVisibilityState(GameObject menu)
    {
        if (menu == null)
        {
            return;
        }

        if (!menu.activeInHierarchy)
        {
            Debug.LogWarning($"Menu '{menu.name}' is activeSelf but not activeInHierarchy. A parent object is disabled.");
            return;
        }

        CanvasGroup canvasGroup = menu.GetComponent<CanvasGroup>();
        if (canvasGroup != null && (canvasGroup.alpha <= 0f || !canvasGroup.interactable || !canvasGroup.blocksRaycasts))
        {
            Debug.LogWarning($"Menu '{menu.name}' has CanvasGroup settings that may hide or block interaction. alpha={canvasGroup.alpha}, interactable={canvasGroup.interactable}, blocksRaycasts={canvasGroup.blocksRaycasts}");
        }
    }
    
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
