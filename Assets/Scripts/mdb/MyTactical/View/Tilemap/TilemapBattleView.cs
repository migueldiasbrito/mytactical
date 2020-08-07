using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;
using UnityEngine.UI;

namespace mdb.MyTactial.View.TilemapView
{
    public class TilemapBattleView : MonoBehaviour
    {
        public static TilemapBattleView instance;

        public Dictionary<Cell, Vector3Int> CellPositions { get; private set; }
        public Dictionary<Vector3Int, Cell> ReachableCells { get; private set; }

        public Tilemap HighlightTilemap = null;

        public Tile HighlightTile = null;

        public Vector3Int[] CellPositionsBuilder = new Vector3Int[0];

        public GameObject ActionsMenu;
        public Button AttackButton;
        public Button NoActionButton;

        public Button MessageButton;
        public Text MessageText;

        private Queue<string> _messages = new Queue<string>();

        [SerializeField]
        private float _fadeTime = 1f;

        [SerializeField]
        private Color HighlightInitialColor = new Color(1f, 1f, 1f, 0.9f);

        [SerializeField]
        private Color _highlightFinalColor = new Color(1f, 1f, 1f, 0.1f);

        private float _fadeDeltaTime;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            ReachableCells = new Dictionary<Vector3Int, Cell>();
            _fadeDeltaTime = 0;

            BattleStateMachine.instance.BuildMap.OnExit += OnBuildMapExit;
            BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;
            BattleStateMachine.instance.MoveUnit.OnExit += OnMoveUnitExit;

            BattleStateMachine.instance.SelectAction.OnEnter += OnSelectAction;
            BattleStateMachine.instance.SelectAction.OnExit += OnSelectActionExit;
            BattleStateMachine.instance.Attack.OnEnter += OnAttack;
            BattleStateMachine.instance.UnitDefeated.OnEnter += OnUnitDefeated;
            BattleStateMachine.instance.EndBattle.OnEnter += OnEndBattle;

            BattleStateMachine.instance.EndBattle.OnClickEvent += OnEndBattleClick;

            AttackButton.onClick.AddListener(Attack);
            NoActionButton.onClick.AddListener(NoAction);

            MessageButton.onClick.AddListener(NextMessage);
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
            CellPositions = new Dictionary<Cell, Vector3Int>();
            for (int cellIndex = 0; cellIndex < BattleController.instance.Battle.Cells.Length; cellIndex++)
            {
                if(cellIndex >= CellPositionsBuilder.Length)
                {
                    Debug.LogError("Not enough positions configured for battle cells.");
                    break;
                }

                CellPositions.Add(BattleController.instance.Battle.Cells[cellIndex], CellPositionsBuilder[cellIndex]);
            }
        }

        private void OnMoveUnit()
        {
            foreach (Cell cell in BattleController.instance.CurrentUnitReachableCells)
            {
                ReachableCells.Add(CellPositions[cell], cell);
                HighlightTilemap.SetTile(CellPositions[cell], HighlightTile);
            }
        }

        private void OnMoveUnitExit()
        {
            foreach (Cell cell in BattleController.instance.CurrentUnitReachableCells)
            {
                HighlightTilemap.SetTile(CellPositions[cell], null);
            }

            ReachableCells.Clear();
        }

        private void OnSelectAction()
        {
            if (!BattleController.instance.CurrentUnit.Team.IsAIControlled)
            {
                AttackButton.gameObject.SetActive(BattleController.instance.HasTargets());
                ActionsMenu.SetActive(true);
            }
        }

        private void OnSelectActionExit()
        {
            ActionsMenu.SetActive(false);
        }

        private void OnAttack()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Name + " attacks " + BattleController.instance.CurrentTarget.Name);
            NextMessage();
        }

        private void OnUnitDefeated()
        {
            _messages.Enqueue(BattleController.instance.CurrentTarget.Name + " defeated");
            NextMessage();
        }

        private void OnEndBattle()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Team.Name + " won");
            NextMessage();
        }

        private void OnEndBattleClick(object obj)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Attack()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.SELECT_TARGET);
        }

        private void NoAction()
        {
            BattleStateMachine.instance.AddTransition(BattleStateMachine.instance.NO_ACTION);
        }

        private void NextMessage()
        {
            if (_messages.Count > 0)
            {
                MessageText.text = _messages.Dequeue();
                MessageText.gameObject.SetActive(true);

                MessageButton.gameObject.SetActive(true);
            }
            else
            {
                MessageText.gameObject.SetActive(false);
                MessageButton.gameObject.SetActive(false);

                BattleStateMachine.instance.OnClick(null);
            }
        }
    }
}