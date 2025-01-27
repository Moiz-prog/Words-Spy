using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using TMPro;
using System;
using System.Text.RegularExpressions;

[CustomEditor(typeof(BoardData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class BoardDataDrawer : Editor
{
    private BoardData GameDataInstance => target as BoardData;
    private ReorderableList _dataList;

    private void OnEnable()
    {
        InitializeReordableList(ref _dataList, "SearchWords", "Searching Words");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameDataInstance.timeInSeconds = EditorGUILayout.FloatField("Max Game Time (in Seconds)", GameDataInstance.timeInSeconds);

        DrawColumnsRowsInputFields();
        EditorGUILayout.Space();
        ConvertToUpperButton();

        if (GameDataInstance.Board != null && GameDataInstance.Columns > 0 && GameDataInstance.Rows > 0)
        {
            DrawBoardTable();
        }

        GUILayout.BeginHorizontal();

        ClearBoardButton();
        FillUpWithRandomLettersButton();

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _dataList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(GameDataInstance);
        }
    }

    private void DrawColumnsRowsInputFields()
    {
        var columnsTemp = GameDataInstance.Columns;
        var rowsTemp = GameDataInstance.Rows;

        GameDataInstance.Columns = EditorGUILayout.IntField("Columns", GameDataInstance.Columns);
        GameDataInstance.Rows = EditorGUILayout.IntField("Rows", GameDataInstance.Rows);

        if ((GameDataInstance.Columns != columnsTemp || GameDataInstance.Rows != rowsTemp) && GameDataInstance.Columns > 0 && GameDataInstance.Rows > 0)
        {
            GameDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        if (GameDataInstance.Board == null || GameDataInstance.Board.Length != GameDataInstance.Columns)
        {
            GameDataInstance.CreateNewBoard();
        }

        var tableStyle = new GUIStyle("box")
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = { left = 32 }
        };

        var headerColumnStyle = new GUIStyle { fixedWidth = 50 };
        var columnStyle = new GUIStyle { fixedWidth = 50 };
        var rowStyle = new GUIStyle
        {
            fixedHeight = 25,
            fixedWidth = 40,
            alignment = TextAnchor.MiddleCenter
        };

        var textFieldStyle = new GUIStyle
        {
            normal =
            {
                background = Texture2D.grayTexture,
                textColor = Color.white
            },
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.BeginHorizontal(tableStyle);

        for (var x = 0; x < GameDataInstance.Columns; x++)
        {
            if (GameDataInstance.Board[x] == null)
            {
                GameDataInstance.Board[x] = new BoardData.BoardRow(GameDataInstance.Rows);
            }

            EditorGUILayout.BeginVertical(x == -1 ? headerColumnStyle : columnStyle);
            for (var y = 0; y < GameDataInstance.Rows; y++)
            {
                if (GameDataInstance.Board[x].Row == null || GameDataInstance.Board[x].Row.Length != GameDataInstance.Rows)
                {
                    GameDataInstance.Board[x].CreateRow(GameDataInstance.Rows);
                }

                EditorGUILayout.BeginHorizontal(rowStyle);

                var character = GameDataInstance.Board[x].Row[y];
                if (character.Length > 1)
                {
                    character = character.Substring(0, 1);
                }

                GameDataInstance.Board[x].Row[y] = EditorGUILayout.TextArea(character, textFieldStyle);

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void InitializeReordableList(ref ReorderableList list, string propertyName, string listLabel)
   {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName), true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        list.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var property = serializedObject.FindProperty(propertyName);

            if (property != null && index >= 0 && index < property.arraySize) // Ensure index is within bounds
            {
                var element = property.GetArrayElementAtIndex(index);
                rect.y += 2;

                if (element != null)
                {
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("Word"),
                        GUIContent.none
                    );
                }
            }
            // else
            // {
            //     Debug.LogWarning("Attempted to access an invalid index or null element in the ReorderableList.");
            // }
    };
}

    private void ConvertToUpperButton()
    {
        if (GUILayout.Button("To Upper"))
        {
            for (var i = 0; i < GameDataInstance.Columns; i++)
            {
                for (var j = 0; j < GameDataInstance.Rows; j++)
                {
                    var errorCounter = Regex.Matches(GameDataInstance.Board[i].Row[j], @"[a-z]").Count;

                    if (errorCounter > 0)
                    {
                        GameDataInstance.Board[i].Row[j] = GameDataInstance.Board[i].Row[j].ToUpper();
                    }
                }

                foreach (var searchWord in GameDataInstance.SearchWords)
                {
                    var errorCounter = Regex.Matches(searchWord.Word, @"[a-z]").Count;

                    if (errorCounter > 0)
                    {
                        searchWord.Word = searchWord.Word.ToUpper();
                    }
                }
            }
        }
    }

    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            for (int i = 0; i < GameDataInstance.Columns; i++)
            {
                for (int j = 0; j < GameDataInstance.Rows; j++)
                {
                    GameDataInstance.Board[i].Row[j] = " ";
                }
            }
        }
    }

    private void FillUpWithRandomLettersButton()
    {
        if (GUILayout.Button("Fill Up With Random"))
        {
            for (int i = 0; i < GameDataInstance.Columns; i++)
            {
                for (int j = 0; j < GameDataInstance.Rows; j++)
                {
                    int errorCounter = Regex.Matches(GameDataInstance.Board[i].Row[j], @"[a-zA-Z]").Count;
                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int index = UnityEngine.Random.Range(0, letters.Length);

                    if (errorCounter == 0)
                    {
                        GameDataInstance.Board[i].Row[j] = letters[index].ToString();
                    }
                }
                
            }
        }
    }
}
