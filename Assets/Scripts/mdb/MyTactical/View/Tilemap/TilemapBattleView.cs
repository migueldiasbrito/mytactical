﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.TilemapView
{
    public class TilemapBattleView : MonoBehaviour
    {
        public Tilemap HighlightTilemap = null;

        public Tile HighlightTile = null;

        public Vector3Int[] CellPositions = new Vector3Int[0];

        [SerializeField]
        private float _fadeTime = 1f;

        [SerializeField]
        private Color HighlightInitialColor = new Color(1f, 1f, 1f, 0.9f);

        [SerializeField]
        private Color _highlightFinalColor = new Color(1f, 1f, 1f, 0.1f);

        private float _fadeDeltaTime;

        private Dictionary<Cell, Vector3Int> _cellPositions;

        private void Start()
        {
            _fadeDeltaTime = 0;

            BattleStateMachine.instance.BuildMap.OnExit += OnBuildMapExit;
            BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;
            BattleStateMachine.instance.MoveUnit.OnExit += OnMoveUnitExit;
        }

        private void Update()
        {
            _fadeDeltaTime += Time.deltaTime;
            HighlightTilemap.color = Color.Lerp(HighlightInitialColor, _highlightFinalColor, _fadeDeltaTime / _fadeTime);

            if (_fadeDeltaTime >= _fadeTime)
            {
                Color temp = HighlightInitialColor;
                HighlightInitialColor = _highlightFinalColor;
                _highlightFinalColor = temp;
                _fadeDeltaTime = 0;
            }
        }

        private void OnBuildMapExit()
        {
            _cellPositions = new Dictionary<Cell, Vector3Int>();
            for (int cellIndex = 0; cellIndex < BattleController.instance.Battle.Cells.Length; cellIndex++)
            {
                if(cellIndex >= CellPositions.Length)
                {
                    Debug.LogError("Not enough positions configured for battle cells.");
                    break;
                }

                _cellPositions.Add(BattleController.instance.Battle.Cells[cellIndex], CellPositions[cellIndex]);
            }
        }

        private void OnMoveUnit()
        {
            foreach (Cell cell in BattleController.instance.CurrentUnitReachableCells)
            {
                HighlightTilemap.SetTile(_cellPositions[cell], HighlightTile);
            }
        }

        private void OnMoveUnitExit()
        {
            foreach (Cell cell in BattleController.instance.CurrentUnitReachableCells)
            {
                HighlightTilemap.SetTile(_cellPositions[cell], null);
            }
        }
    }
}