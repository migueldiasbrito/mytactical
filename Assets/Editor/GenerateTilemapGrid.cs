using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;
using mdb.MyTactial.View.TilemapView;

namespace mdb.MyTactial.EditorTools
{
    public class GenerateTilemapGrid : EditorWindow
    {
        [Serializable]
        public struct UnitBuilder
        {
            public string name;
            public int x;
            public int y;
            public int HealthPoints;
            public int Attack;
            public int Defense;
            public int Movement;
            public int Agility;
            public GameObject Prefab;

            public UnitBuilder(
                string name,
                int x,
                int y,
                int healthPoint,
                int attack,
                int defense,
                int movement,
                int agility
                )
            {
                this.name = name;
                this.x = x;
                this.y = y;
                this.HealthPoints = healthPoint;
                this.Attack = attack;
                this.Defense = defense;
                this.Movement = movement;
                this.Agility = agility;
                this.Prefab = null;
            }
        }

        [Serializable]
        public struct Team
        {
            public string Name;
            public bool AIControlled;
            public UnitBuilder[] Units;
        }

        public int Rows = 20;
        public int Columns = 14;
        public Tile DefaultTile = null;
        public Tile HighlightTile = null;

        public Team[] Teams = new Team[] {
            new Team{
                Name = "Gleaming Squad",
                AIControlled = false,
                Units = new UnitBuilder[] {
                    new UnitBuilder("Max",  7, 3, 12, 14, 4, 6, 4),
                    new UnitBuilder("Hans", 8, 3, 14, 14, 5, 5, 6),
                    new UnitBuilder("Tao",  7, 2, 10,  8, 4, 5, 6),
                    new UnitBuilder("Lowe", 8, 2, 11, 10, 5, 5, 5)
                }
            },
            new Team{
                Name = "Signfaust",
                AIControlled = true,
                Units = new UnitBuilder[] {
                    new UnitBuilder("Sign Knight",  7, 16, 14, 16, 7, 7, 7),
                    new UnitBuilder("Dark Dwarf",   6, 15, 12, 12, 8, 4, 5),
                    new UnitBuilder("Dark Dwarf",   7, 15, 12, 12, 8, 4, 5),
                    new UnitBuilder("Goblin",       6, 14, 12,  9, 6, 5, 5),
                    new UnitBuilder("Goblin",       7, 14, 12,  9, 6, 5, 5),
                    new UnitBuilder("Goblin",       9, 11, 12,  9, 6, 5, 5),
                    new UnitBuilder("Goblin",       8, 10, 12,  9, 6, 5, 5),
                    new UnitBuilder("Goblin",       7, 10, 12,  9, 6, 5, 5)
                }
            }
        };

        Vector2 _scrollPos;

        [MenuItem("MyTactial/Generate Grid/Generate Tilemap Grid")]
        static void OpenWindow()
        {
            GetWindow(typeof(GenerateTilemapGrid), false, "Generate Tilemap Grid");
        }

        void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            SerializedObject so = new SerializedObject(this);
            SerializedProperty rows = so.FindProperty("Rows");
            SerializedProperty columns = so.FindProperty("Columns");
            SerializedProperty defaultTile = so.FindProperty("DefaultTile");
            SerializedProperty highlightTile = so.FindProperty("HighlightTile");
            SerializedProperty teams = so.FindProperty("Teams");

            EditorGUILayout.PropertyField(rows, true);
            EditorGUILayout.PropertyField(columns, true);
            EditorGUILayout.PropertyField(defaultTile, true);
            EditorGUILayout.PropertyField(highlightTile, true);
            EditorGUILayout.PropertyField(teams, true);
            so.ApplyModifiedProperties();

            if (GUILayout.Button("Generate"))
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

            HashSet<Vector2> testDuplicastes = new HashSet<Vector2>();
            foreach (Team team in Teams)
            {
                foreach (UnitBuilder unitPos in team.Units)
                {
                    if (unitPos.x < 0 || unitPos.x >= Columns || unitPos.y < 0 || unitPos.y >= Rows)
                    {
                        Debug.LogError("Unit with positions outside the grid.");
                        return;
                    }

                    if (!testDuplicastes.Add(new Vector2(unitPos.x, unitPos.y)))
                    {
                        Debug.LogError("Cell with multiple units.");
                        return;
                    }
                }
            }

            GameObject gridGameObject = new GameObject("Grid", typeof(Grid), typeof(BattleController), typeof(TilemapBattleView));
            gridGameObject.GetComponent<Grid>().cellSize = new Vector3(1f, 1f);
            BattleController battleController = gridGameObject.GetComponent<BattleController>();
            TilemapBattleView tilemapBattleView = gridGameObject.GetComponent<TilemapBattleView>();
            tilemapBattleView.HighlightTile = HighlightTile;

            GameObject backgroundGameObject = new GameObject("Background", typeof(Tilemap), typeof(TilemapRenderer));
            backgroundGameObject.transform.SetParent(gridGameObject.transform);
            Tilemap backgroundTilemap = backgroundGameObject.GetComponent<Tilemap>();

            GameObject highlightGameObject = new GameObject("Highlight", typeof(Tilemap), typeof(TilemapRenderer));
            highlightGameObject.transform.SetParent(gridGameObject.transform);
            highlightGameObject.GetComponent<TilemapRenderer>().sortingOrder = 1;
            tilemapBattleView.HighlightTilemap = highlightGameObject.GetComponent<Tilemap>();

            Cell[] cells = new Cell[Rows * Columns];
            Battle.CellAdjacentsBuilder[] celAdjacentBuilders = new Battle.CellAdjacentsBuilder[Rows * Columns];
            tilemapBattleView.CellPositions = new Vector3Int[Rows * Columns];
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Vector3Int position = new Vector3Int(column, row, 0);
                    backgroundTilemap.SetTile(position, DefaultTile);
                    cells[row * Columns + column] = new Cell();
                    tilemapBattleView.CellPositions[row * Columns + column] = position;
                    
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
                    celAdjacentBuilders[row * Columns + column] = new Battle.CellAdjacentsBuilder { AdjacentCells = adjacentCells.ToArray() };
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

                    Unit unit = new Unit(
                        unitBuilder.name,
                        unitBuilder.HealthPoints,
                        unitBuilder.Attack,
                        unitBuilder.Defense,
                        unitBuilder.Movement,
                        unitBuilder.Agility
                    );

                    units[unitIndex] = unit;
                    initialPositions.Add(unitBuilder.x + unitBuilder.y * Columns);

                    GameObject unitGameObject;
                    if (unitBuilder.Prefab != null)
                    {
                        unitGameObject = Instantiate(unitBuilder.Prefab);
                        unitGameObject.name = unitBuilder.name;
                    }
                    else
                    {
                        unitGameObject = new GameObject(unitBuilder.name);
                    }

                    unitGameObject.transform.position =
                        tilemapBattleView.CellPositions[unitBuilder.x + unitBuilder.y * Columns];
                }

                teams[teamIndex].Units = units;
            }

            Battle battle = new Battle(cells, celAdjacentBuilders, teams, initialPositions.ToArray());
            battleController.Battle = battle;
        }
    }
}