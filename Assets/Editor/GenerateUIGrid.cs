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
        public struct Position
        {
            public int x;
            public int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Serializable]
        public struct Team
        {
            public Color Color;
            public Position[] Units;
        }

        private const int TotalAdjacentCells = 4;
        private const int upAdjacentCellIndex = 0;
        private const int downAdjacentCellIndex = 1;
        private const int leftAdjacentCellIndex = 2;
        private const int rightAdjacentCellIndex = 3;

        public int Rows = 10;
        public int Columns = 10;

        Vector2 _scrollPos;

        public Team[] Teams = new Team[] {
            new Team{ Color = Color.blue, Units = new Position[] { new Position(3, 0), new Position(4, 0), new Position(5, 0), new Position(6, 0) } },
            new Team{ Color = Color.red, Units = new Position[] { new Position(3, 9), new Position(4, 9), new Position(5, 9), new Position(6, 9), new Position(4, 8), new Position(5, 8), new Position(4, 7), new Position(5, 7) } }
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

            HashSet<Position> testDuplicastes = new HashSet<Position>();
            foreach (Team team in Teams)
            {
                foreach (Position unitPos in team.Units)
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

            Battle battle = new Battle();
            RectTransform battleGrid = RectTransformTools.CreateStretched("BattleGrid", (RectTransform)canvas.transform, Vector2.zero, Vector2.one);
            Controller.BattleController battleController = battleGrid.gameObject.AddComponent<Controller.BattleController>();
            battleController.Battle = battle;

            UICellView[,] cellViews = new UICellView[Columns, Rows];
            Cell[] cells = new Cell[Columns * Rows];

            float heightPercentage = 1.0f / Rows;
            float widthPercentage = 1.0f / Columns;

            int row, column;
            float miny, minx;
            for (row = 0, miny = 0; row < Rows; row++, miny += heightPercentage)
            {
                for (column = 0, minx = 0; column < Columns; column++, minx += widthPercentage)
                {
                    Cell cell = new Cell(TotalAdjacentCells);

                    if (row > 0)
                    {
                        cell.AdjacentCells[downAdjacentCellIndex] = cellViews[column, row - 1].Cell;
                        cellViews[column, row - 1].Cell.AdjacentCells[upAdjacentCellIndex] = cell;
                    }

                    if (column > 0)
                    {
                        cell.AdjacentCells[leftAdjacentCellIndex] = cellViews[column - 1, row].Cell;
                        cellViews[column - 1, row].Cell.AdjacentCells[rightAdjacentCellIndex] = cell;
                    }

                    cells[row * Rows + column] = cell;

                    RectTransform cellTransform = RectTransformTools.CreateStretched("Cell" + row + column, battleGrid, new Vector2(minx, miny), new Vector2(minx + heightPercentage, miny + widthPercentage));
                    UICellView cellView = cellTransform.gameObject.AddComponent<UICellView>();
                    cellView.Cell = cell;
                    cellViews[column, row] = cellView;

                    cellTransform.gameObject.GetComponent<Image>().color = ((row + column) % 2 == 0) ? Color.white : Color.grey;
                }
            }
            battle.Cells = cells;

            Model.Team[] teams = new Model.Team[Teams.Length];
            for (int teamIndex = 0; teamIndex < Teams.Length; teamIndex++)
            {
                teams[teamIndex] = new Model.Team();
                Unit[] units = new Unit[Teams[teamIndex].Units.Length];
                for (int positionIndex = 0; positionIndex < Teams[teamIndex].Units.Length; positionIndex++)
                {
                    Position position = Teams[teamIndex].Units[positionIndex];

                    Unit unit = new Unit(teams[teamIndex], cellViews[position.x, position.y].Cell);
                    cellViews[position.x, position.y].Cell.UnitEnter(unit);
                    units[positionIndex] = unit;

                    RectTransform unitTransform = RectTransformTools.CreateStretched(
                        "T" + teamIndex + "U" + positionIndex,
                        (RectTransform)cellViews[position.x, position.y].transform,
                        new Vector2(0.1f, 0.1f),
                        new Vector2(0.9f, 0.9f));

                    UIUnitView unitView = unitTransform.gameObject.AddComponent<UIUnitView>();
                    unitView.Unit = unit;

                    unitTransform.gameObject.GetComponent<Image>().color = Teams[teamIndex].Color;
                }
                teams[teamIndex].Units = units;
            }
            battle.Teams = teams;
        }
    }
}
