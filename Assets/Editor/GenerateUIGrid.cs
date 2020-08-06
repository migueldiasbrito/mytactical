using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Model;
using mdb.MyTactial.View.UIView;
using mdb.Tools;

namespace mdb.MyTactial.EditorTools
{
    public class GenerateUIGrid : EditorWindow
    {
        [Serializable]
        public struct UnitBuilder
        {
            public string name;
            public int x;
            public int y;

            public UnitBuilder(string name, int x, int y)
            {
                this.name = name;
                this.x = x;
                this.y = y;
            }
        }

        [Serializable]
        public struct Team
        {
            public string Name;
            public bool AIControlled;
            public Color Color;
            public UnitBuilder[] Units;
        }

        public int Rows = 10;
        public int Columns = 10;

        Vector2 _scrollPos;

        public Team[] Teams = new Team[] {
            new Team{
                Name = "Blue",
                AIControlled = false,
                Color = Color.blue,
                Units = new UnitBuilder[] {
                    new UnitBuilder("Blue Unit", 3, 0),
                    new UnitBuilder("Blue Unit", 4, 0),
                    new UnitBuilder("Blue Unit", 5, 0),
                    new UnitBuilder("Blue Unit", 6, 0)
                }
            },
            new Team{
                Name = "Red",
                AIControlled = true,
                Color = Color.red,
                Units = new UnitBuilder[] {
                    new UnitBuilder("Red Unit", 3, 9),
                    new UnitBuilder("Red Unit", 4, 9),
                    new UnitBuilder("Red Unit", 5, 9),
                    new UnitBuilder("Red Unit", 6, 9),
                    new UnitBuilder("Red Unit", 4, 8),
                    new UnitBuilder("Red Unit", 5, 8),
                    new UnitBuilder("Red Unit", 4, 7),
                    new UnitBuilder("Red Unit", 5, 7)
                }
            }
        };

        [MenuItem("MyTactial/Generate Grid/Generate UI Grid")]
        static void OpenWindow()
        {
            GetWindow(typeof(GenerateUIGrid), false, "Generate UI Grid");
        }

        void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            SerializedObject so = new SerializedObject(this);
            SerializedProperty rows = so.FindProperty("Rows");
            SerializedProperty columns = so.FindProperty("Columns");
            SerializedProperty teams = so.FindProperty("Teams");

            EditorGUILayout.PropertyField(rows, true);
            EditorGUILayout.PropertyField(columns, true);
            EditorGUILayout.PropertyField(teams, true);
            so.ApplyModifiedProperties();

            if(GUILayout.Button("Generate"))
            {
                GenerateGrid();
            }

            EditorGUILayout.EndScrollView();
        }

        void GenerateGrid()
        {
            if (Rows < 1 || Columns < 1)
            {
                Debug.LogError("Must have positive number of rows and columns to generate a new grid");
                return;
            }

            HashSet<UnitBuilder> testDuplicastes = new HashSet<UnitBuilder>();
            foreach (Team team in Teams)
            {
                foreach (UnitBuilder unitPos in team.Units)
                {
                    if (unitPos.x < 0 || unitPos.x >= Columns || unitPos.y < 0 || unitPos.y >= Columns)
                    {
                        Debug.LogError("Unit with positions outside the grid.");
                        return;
                    }

                    if (!testDuplicastes.Add(unitPos))
                    {
                        Debug.LogError("Cell with multiple units.");
                        return;
                    }
                }
            }

            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("Would you be so kind to create a canvas for me first?");
                return;
            }

            RectTransform battleGrid = RectTransformTools.CreateStretched("BattleGrid", (RectTransform)canvas.transform, Vector2.zero, Vector2.one);
            UIBattleView battleView = battleGrid.gameObject.AddComponent<UIBattleView>();
            Controller.BattleController battleController = battleGrid.gameObject.GetComponent<Controller.BattleController>();

            battleView.cellViews = new UICellView[Columns * Rows];
            UICellView[,] cellViews = new UICellView[Columns, Rows];
            Cell[] cells = new Cell[Columns * Rows];
            Battle.CellAdjacentsBuilder[] celAdjacentBuilders = new Battle.CellAdjacentsBuilder[Columns * Rows];

            float heightPercentage = 1.0f / Rows;
            float widthPercentage = 1.0f / Columns;

            int row, column;
            float miny, minx;
            for (row = 0, miny = 0; row < Rows; row++, miny += heightPercentage)
            {
                for (column = 0, minx = 0; column < Columns; column++, minx += widthPercentage)
                {
                    Cell cell = new Cell();

                    List<int> adjacentCells = new List<int>();

                    if (row > 0 && Rows > 1)
                    {
                        adjacentCells.Add((row - 1) * Columns + column);
                    }

                    if (row < Rows - 1 && Rows > 1)
                    {
                        adjacentCells.Add((row + 1) * Columns + column);
                    }

                    if (column > 0 && Columns > 1)
                    {
                        adjacentCells.Add(row * Columns + column - 1);
                    }

                    if (column < Columns - 1 && Columns > 1)
                    {
                        adjacentCells.Add(row * Columns + column + 1);
                    }

                    celAdjacentBuilders[row * Rows + column] = new Battle.CellAdjacentsBuilder { AdjacentCells = adjacentCells.ToArray() };

                    cells[row * Columns + column] = cell;

                    RectTransform cellTransform = RectTransformTools.CreateStretched("Cell" + row + column, battleGrid, new Vector2(minx, miny), new Vector2(minx + heightPercentage, miny + widthPercentage));
                    UICellView cellView = cellTransform.gameObject.AddComponent<UICellView>();
                    cellView.CellIndex = row * Columns + column;

                    battleView.cellViews[row * Columns + column] = cellView;
                    cellViews[column, row] = cellView;

                    cellTransform.gameObject.GetComponent<Image>().color = ((row + column) % 2 == 0) ? Color.white : Color.grey;

                    Button button = cellTransform.gameObject.GetComponent<Button>();
                    button.transition = Selectable.Transition.None;
                    button.enabled = false;
                }
            }

            Model.Team[] teams = new Model.Team[Teams.Length];
            List<int> initialPositions = new List<int>();
            for (int teamIndex = 0; teamIndex < Teams.Length; teamIndex++)
            {
                teams[teamIndex] = new Model.Team(Teams[teamIndex].Name, Teams[teamIndex].AIControlled);
                Unit[] units = new Unit[Teams[teamIndex].Units.Length];
                for (int unitIndex = 0; unitIndex < Teams[teamIndex].Units.Length; unitIndex++)
                {
                    UnitBuilder unitBuilder = Teams[teamIndex].Units[unitIndex];

                    Unit unit = new Unit(unitBuilder.name);
                    units[unitIndex] = unit;
                    initialPositions.Add(unitBuilder.x + unitBuilder.y * Columns);

                    RectTransform unitTransform = RectTransformTools.CreateStretched(
                        "T" + teamIndex + "U" + unitIndex,
                        (RectTransform)cellViews[unitBuilder.x, unitBuilder.y].transform,
                        new Vector2(0.2f, 0.2f),
                        new Vector2(0.8f, 0.8f));

                    UIUnitView unitView = unitTransform.gameObject.AddComponent<UIUnitView>();
                    unitView.TeamIndex = teamIndex;
                    unitView.UnitIndex = unitIndex;

                    unitTransform.gameObject.GetComponent<Image>().color = Teams[teamIndex].Color;
                }
                teams[teamIndex].Units = units;
            }

            Battle battle = new Battle(cells, celAdjacentBuilders, teams, initialPositions.ToArray());
            battleController.Battle = battle;

            RectTransform actionsMenu = RectTransformTools.CreateStretched("MenuActions", (RectTransform)canvas.transform, Vector2.zero, new Vector2(1, 0.1f));
            battleView.ActionsMenu = actionsMenu.gameObject;

            battleView.AttackButton = CreateButtonHelper("ATTACK", actionsMenu, new Vector2(0.15f, 0.1f), new Vector2(0.45f, 0.9f));
            battleView.NoActionButton = CreateButtonHelper("NO ACTION", actionsMenu, new Vector2(0.55f, 0.1f), new Vector2(0.85f, 0.9f));

            actionsMenu.gameObject.SetActive(false);

            battleView.MessageButton = CreateButtonHelper("MessageBox", (RectTransform)canvas.transform, new Vector2(0.1f, 0.01f), new Vector2(0.9f, 0.14f));
            battleView.MessageButton.gameObject.SetActive(false);
            battleView.MessageText = battleView.MessageButton.GetComponentInChildren<Text>();
            battleView.MessageText.gameObject.SetActive(false);
        }

        private Button CreateButtonHelper(string text, RectTransform parent, Vector2 minAnchor, Vector2 maxAnchor)
        {
            RectTransform gameObject = RectTransformTools.CreateStretched(text, parent, minAnchor, maxAnchor);
            Button button = gameObject.gameObject.AddComponent<Button>();
            
            Image image = gameObject.gameObject.AddComponent<Image>();
            image.color = new Color(0.75f, 0.75f, 0.75f, 0.85f);
            button.image = image;

            RectTransform textGameObject = RectTransformTools.CreateStretched("Text", gameObject, Vector2.zero, Vector2.one);
            Text buttonText = textGameObject.gameObject.AddComponent<Text>();
            buttonText.text = text;
            buttonText.color = Color.black;
            buttonText.alignment = TextAnchor.MiddleCenter;

            return button;
        }
    }
}
