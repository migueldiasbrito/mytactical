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

        public GameObject[] TeamUnitInfo;
        public Text[] TeamUnitName;
        public Text[] TeamUnitHP;

        private Queue<string> _messages = new Queue<string>();

        [SerializeField]
        private float _fadeTime = 1f;

        [SerializeField]
        private Color _highlightInitialColor = new Color(1f, 1f, 1f, 0.9f);

        [SerializeField]
        private Color _highlightFinalColor = new Color(1f, 1f, 1f, 0.1f);

        [SerializeField]
        private float _inputCooldown = 0.5f;

        private float _fadeDeltaTime;
        private float _cooldownDeltaTime = 0;

        private Unit _target = null;
        private bool _selectingTarget = false;
        private Button _activeButton;

        public void ShowInfo(int teamIndex, string name, int currentHealthPoints, int totalHealthPoints)
        {
            if(TeamUnitInfo != null && TeamUnitInfo.Length > teamIndex && TeamUnitInfo[teamIndex] != null)
            {
                TeamUnitInfo[teamIndex].SetActive(true);

                if (TeamUnitName != null && TeamUnitName.Length > teamIndex && TeamUnitName[teamIndex] != null)
                {
                    TeamUnitName[teamIndex].text = name;
                }

                if (TeamUnitHP != null && TeamUnitHP.Length > teamIndex && TeamUnitHP[teamIndex] != null)
                {
                    TeamUnitHP[teamIndex].text = currentHealthPoints + "/" + totalHealthPoints;
                }
            }
        }

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
            BattleStateMachine.instance.SelectTarget.OnEnter += OnSelectTarget;
            BattleStateMachine.instance.Attack.OnEnter += OnAttack;
            BattleStateMachine.instance.UnitDefeated.OnEnter += OnUnitDefeated;
            BattleStateMachine.instance.EndUnitTurn.OnEnter += OnEndUnitTurn;
            BattleStateMachine.instance.EndBattle.OnEnter += OnEndBattle;

            BattleStateMachine.instance.EndBattle.OnClickEvent += OnEndBattleClick;

            AttackButton.onClick.AddListener(Attack);
            NoActionButton.onClick.AddListener(NoAction);

            MessageButton.onClick.AddListener(NextMessage);
        }

        private void Update()
        {
            _fadeDeltaTime += Time.deltaTime;
            HighlightTilemap.color = Color.Lerp(_highlightInitialColor, _highlightFinalColor, _fadeDeltaTime / _fadeTime);

            if (_fadeDeltaTime >= _fadeTime)
            {
                Color temp = _highlightInitialColor;
                _highlightInitialColor = _highlightFinalColor;
                _highlightFinalColor = temp;
                _fadeDeltaTime = 0;
            }

            _cooldownDeltaTime -= Time.deltaTime;

            if (_cooldownDeltaTime <= 0)
            {
                if (MessageButton.gameObject.activeSelf && Input.GetAxisRaw("Fire1") == 1)
                {
                    MessageButton.onClick.Invoke();
                    return;
                }

                if (ActionsMenu.activeSelf)
                {
                    if (Input.GetAxisRaw("Fire1") == 1)
                    {
                        _activeButton.onClick.Invoke();
                        _cooldownDeltaTime = _inputCooldown;
                        return;
                    }

                    float verticalInput = Input.GetAxisRaw("Vertical");
                    float horizontalInput = Input.GetAxisRaw("Horizontal");

                    if (_activeButton == AttackButton && (verticalInput == 1 || horizontalInput == 1))
                    {
                        _activeButton.transform.localScale = new Vector3(1f, 1f, 1f);
                        _activeButton = NoActionButton;
                        _activeButton.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                        _cooldownDeltaTime = _inputCooldown;
                        return;
                    }
                    else if (_activeButton == AttackButton && (verticalInput == -1 || horizontalInput == -1))
                    {
                        _activeButton.transform.localScale = new Vector3(1f, 1f, 1f);
                        _activeButton = AttackButton;
                        _activeButton.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                        _cooldownDeltaTime = _inputCooldown;
                        return;
                    }
                }

                if (_selectingTarget)
                {
                    if (Input.GetAxisRaw("Fire1") == 1)
                    {
                        BattleStateMachine.instance.OnClick(_target);
                        _target.SetState(Unit.State.Idle);
                        _target = null;
                        _selectingTarget = false;
                        _cooldownDeltaTime = _inputCooldown;
                        return;
                    }

                    if (BattleController.instance.CurrentTargetUnits.Length > 0)
                    {
                        float verticalInput = Input.GetAxisRaw("Vertical");
                        float horizontalInput = Input.GetAxisRaw("Horizontal");

                        if (verticalInput == 1 || horizontalInput == 1)
                        {
                            List<Unit> targetUnits = new List<Unit>(BattleController.instance.CurrentTargetUnits);
                            int unitIndex = targetUnits.IndexOf(_target);
                            unitIndex = (unitIndex + 1) % targetUnits.Count;

                            _target.SetState(Unit.State.Idle);
                            _target = targetUnits[unitIndex];
                            _target.SetState(Unit.State.Target);

                            _cooldownDeltaTime = _inputCooldown;
                        }
                        else if (verticalInput == -1 || horizontalInput == -1)
                        {
                            List<Unit> targetUnits = new List<Unit>(BattleController.instance.CurrentTargetUnits);
                            int unitIndex = targetUnits.IndexOf(_target);
                            unitIndex = (unitIndex - 1) % targetUnits.Count;
                            unitIndex = unitIndex < 0 ? unitIndex + targetUnits.Count : unitIndex;

                            _target.SetState(Unit.State.Idle);
                            _target = targetUnits[unitIndex];
                            _target.SetState(Unit.State.Target);

                            _cooldownDeltaTime = _inputCooldown;
                        }
                    }
                }
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
                if (BattleController.instance.HasTargets())
                {
                    AttackButton.gameObject.SetActive(true);
                    _activeButton = AttackButton;
                }
                else
                {
                    AttackButton.gameObject.SetActive(false);
                    _activeButton = NoActionButton;
                }

                _activeButton.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                ActionsMenu.SetActive(true);
                _cooldownDeltaTime = _inputCooldown;
            }
        }

        private void OnSelectActionExit()
        {
            if (!BattleController.instance.CurrentUnit.Team.IsAIControlled)
            {
                _activeButton.transform.localScale = new Vector3(1f, 1f, 1f);
                ActionsMenu.SetActive(false);
            }
        }

        private void OnSelectTarget()
        {
            if (!BattleController.instance.CurrentUnit.Team.IsAIControlled && BattleController.instance.HasTargets())
            {
                _selectingTarget = true;
                _target = BattleController.instance.CurrentTargetUnits[0];
                _target.SetState(Unit.State.Target);
            }
        }

        private void OnAttack()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Name + " attacks!");
            _messages.Enqueue("Inflicts " + BattleController.instance.CurrentDamage + " points of damage on " + BattleController.instance.CurrentTarget.Name);
            NextMessage();
        }

        private void OnUnitDefeated()
        {
            _messages.Enqueue(BattleController.instance.CurrentTarget.Name + " defeated");
            NextMessage();
        }

        private void OnEndUnitTurn()
        {
            foreach (GameObject gameObject in TeamUnitInfo)
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnEndBattle()
        {
            _messages.Enqueue(BattleController.instance.CurrentUnit.Team.Name + " won");
            NextMessage();
        }

        private void OnEndBattleClick(object obj)
        {
            SceneManager.LoadScene(0);
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
            _cooldownDeltaTime = _inputCooldown;
        }
    }
}