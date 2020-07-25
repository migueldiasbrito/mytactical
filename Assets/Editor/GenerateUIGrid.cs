using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MyTactial.EditorTools
{
    public class GenerateUIGrid : EditorWindow
    {
        int rows = 10;
        int columns = 10;

        [MenuItem("MyTactial/Generate Grid/Generate UI Grid")]
        static void OpenWindow()
        {
            GetWindowWithRect(typeof(GenerateUIGrid), new Rect(0, 0, 200, 75) ,false, "Generate UI Grid");
        }

        void OnGUI()
        {
            rows = EditorGUILayout.IntField("Number of rows", rows);
            columns = EditorGUILayout.IntField("Number of columns", columns);

            if(GUILayout.Button("Generate"))
            {
                GenerateGrid();
                Close();
            }
        }

        void GenerateGrid()
        {
            if (rows < 1 || columns < 1)
            {
                Debug.LogError("Must have positive number of rows and columns to generate a new grid");
                return;
            }

            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("Would you be so kind to create a canvas for me first?");
                return;
            }

            Model.Battle battle = new Model.Battle(rows * columns);
            RectTransform battleGrid = Tools.RectTransformTools.CreateStretched("BattleGrid", (RectTransform)canvas.transform, Vector2.zero, Vector2.one);
            Controller.BattleController battleController = battleGrid.gameObject.AddComponent<Controller.BattleController>();
            battleController.Battle = battle;

            float heightPercentage = 1.0f / rows;
            float widthPercentage = 1.0f / columns;

            int row, column;
            float miny, minx;
            for (row = 0, miny = 0; row < rows; row++, miny += heightPercentage)
            {
                for (column = 0, minx = 0; column < columns; column++, minx += widthPercentage)
                {
                    Model.Cell cell = new Model.Cell();

                    RectTransform cellTransform = Tools.RectTransformTools.CreateStretched("Cell" + row + column, battleGrid, new Vector2(minx, miny), new Vector2(minx + heightPercentage, miny + widthPercentage));
                    View.UIView.UICellView cellView = cellTransform.gameObject.AddComponent<View.UIView.UICellView>();

                    cellTransform.gameObject.GetComponent<Image>().color = ((row + column) % 2 == 0) ? Color.white : Color.grey;

                    cellView.Cell = cell;
                    battleController.Battle.cells[row * rows + column] = cell;
                }
            }
        }
    }
}
