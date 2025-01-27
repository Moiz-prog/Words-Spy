using UnityEngine;

public class GameDataSelector : MonoBehaviour
{
    public GameData currentGamaData;
    public GameLevelData levelData;

    void Awake()
    {
        SelectSequentialBoardData();
    }

    private void SelectSequentialBoardData()
    {
        foreach (var data in levelData.data)
        {
            if (data.categoryName == currentGamaData.selectedCategoryName)
            {
                if (data.boardData == null || data.boardData.Count == 0)
                {
                    // Debug.LogError($"No board data available for category: {data.categoryName}");
                    continue;
                }

                var boardIndex = DataSaver.ReadCategoryCurrentIndexValues(currentGamaData.selectedCategoryName);
                Debug.Log($"Category: {data.categoryName}, Board Count: {data.boardData.Count}, Board Index: {boardIndex}");

                if (boardIndex >= 0 && boardIndex < data.boardData.Count)
                {
                    currentGamaData.selectedBoardData = data.boardData[boardIndex];
                }
                else
                {
                    // Debug.LogWarning("Invalid board index. Selecting a random board.");
                    var randomIndex = Random.Range(0, data.boardData.Count);
                    currentGamaData.selectedBoardData = data.boardData[randomIndex];
                }
            }
        }
    }
}
