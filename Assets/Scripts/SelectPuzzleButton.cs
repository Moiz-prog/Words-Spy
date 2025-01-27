using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectPuzzleButton : MonoBehaviour
{
    public GameData gameData;
    public GameLevelData levelData;
    public Text categoryText;
    public Image progressBarFilling;

    private string gameSceneName = "GameScene";

    private bool _levelLocked;

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        UpdateButtonInformation();

        // Disable button if the level is locked
        button.interactable = !_levelLocked;
    }

    void UpdateButtonInformation()
    {
        int currentIndex = -1;
        int totalBoards = 0;

        foreach (var data in levelData.data)
        {
            if (data.categoryName == gameObject.name)
            {
                // Read current index for this category
                currentIndex = DataSaver.ReadCategoryCurrentIndexValues(data.categoryName);
                totalBoards = data.boardData.Count;

                // Initialize index if no data is saved
                if (currentIndex < 0)
                {
                    currentIndex = 0; // Default to the first board
                    DataSaver.SaveCategoryData(data.categoryName, currentIndex);
                }

                int categoryIndex = levelData.data.IndexOf(data);

                if (categoryIndex > 0)
                {
                    var previousCategory = levelData.data[categoryIndex - 1];
                    int previousCategoryIndex = DataSaver.ReadCategoryCurrentIndexValues(previousCategory.categoryName);
                    _levelLocked = previousCategoryIndex < previousCategory.boardData.Count;
                }
                else
                {
                    _levelLocked = false;
                }
                
            }
        }

        // Update UI elements
        categoryText.text = _levelLocked ? string.Empty : $"{currentIndex}/{totalBoards}";
        progressBarFilling.fillAmount = (totalBoards > 0) ? (float)currentIndex / totalBoards : 0f;
    }

    private void OnButtonClick()
    {
        // Set the selected category and load the game scene
        gameData.selectedCategoryName = gameObject.name;
        SceneManager.LoadScene(gameSceneName);
    }
}
