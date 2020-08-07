using System.Collections.Generic;
using UnityEngine;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.TilemapView
{
    [RequireComponent(typeof(GridMovement))]
    public class TilemapUnitView : UnitView
    {
        public int TeamIndex;
        public int UnitIndex;

        [SerializeField]
        private float _fadeTime = 1f;

        [SerializeField]
        private Color _highlightInitialColor = new Color(1f, 1f, 1f, 1f);

        [SerializeField]
        private Color _highlightFinalColor = new Color(0.1f, 0.1f, 0.1f, 1f);

        [SerializeField]
        private float _inputCooldown = 0.5f;

        private Unit _unit;
        private GridMovement _gridMovement;
        private Queue<Vector2> _path;
        private bool _moving = false;
        private Cell _lastCell;
        private bool _controlUnit = false;
        private SpriteRenderer _spriteRenderer;
        private Color _defaultColor = new Color(1f, 1f, 1f, 0.9f);
        private bool _targeted = false;
        private float _fadeDeltaTime = 0;
        private float _cooldownDeltaTime = 0;
        private Animator _animator;

        public override void DoPath(Cell[] cells)
        {
            _moving = true;
            _lastCell = cells.Length > 0 ? cells[cells.Length - 1] : _unit.Cell;

            foreach(Cell cell in cells)
            {
                Vector3Int target = TilemapBattleView.instance.CellPositions[cell];
                _path.Enqueue(new Vector2(target.x, target.y));
            }
        }
        public void DoPath(Vector3Int target, Cell cell)
        {
            _moving = true;
            _lastCell = cell;
            _path.Enqueue(new Vector2(target.x, target.y));
        }

        private void Start()
        {
            _unit = BattleController.instance.Battle.Teams[TeamIndex].Units[UnitIndex];
            _unit.StateChangedCallback += UnitStateChanged;

            _gridMovement = GetComponent<GridMovement>();
            _path = new Queue<Vector2>();

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _defaultColor = _spriteRenderer.color;

            _animator = GetComponentInChildren<Animator>();

            BattleStateMachine.instance.MoveUnit.OnEnter += OnMoveUnit;
        }

        private void Update()
        {
            if (_moving && _gridMovement.MoveTo == null)
            {
                if (_path.Count > 0)
                {
                    Vector2 nextPosition = _path.Dequeue();
                    _gridMovement.MoveTo = nextPosition;

                    if (nextPosition.y != transform.position.y)
                    {
                        _animator.SetTrigger(nextPosition.y > transform.position.y ? "Back" : "Front");
                    }
                    else if(nextPosition.x != transform.position.x)
                    {
                        _animator.SetTrigger(nextPosition.x > transform.position.x ? "Right" : "Left");
                    }
                }
                else
                {
                    _moving = false;

                    if (!_controlUnit)
                    {
                        BattleStateMachine.instance.OnClick(_lastCell);
                        _lastCell = null;
                    }
                }
            }

            if(_targeted)
            {
                _fadeDeltaTime += Time.deltaTime;
                _spriteRenderer.color = Color.Lerp(_highlightInitialColor, _highlightFinalColor, _fadeDeltaTime / _fadeTime);

                if (_fadeDeltaTime >= _fadeTime)
                {
                    Color temp = _highlightInitialColor;
                    _highlightInitialColor = _highlightFinalColor;
                    _highlightFinalColor = temp;
                    _fadeDeltaTime = 0;
                }
            }
            else if (_spriteRenderer.color != _defaultColor)
            {
                _spriteRenderer.color = _defaultColor;
                _fadeDeltaTime = 0;
            }

            if (_controlUnit && !_moving)
            {
                _cooldownDeltaTime -= Time.deltaTime;

                Vector3Int position = new Vector3Int(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y),
                    Mathf.RoundToInt(transform.position.z)
                );

                if (Input.GetAxisRaw("Fire1") == 1 && _cooldownDeltaTime <= 0)
                {
                    if (_lastCell.Unit == null || _lastCell.Unit == _unit)
                    {
                        BattleStateMachine.instance.OnClick(TilemapBattleView.instance.ReachableCells[position]);
                        _controlUnit = false;
                    }
                    return;
                }

                float verticalInput = Input.GetAxisRaw("Vertical");

                if (Mathf.Abs(verticalInput) == 1)
                {
                    position += new Vector3Int(0, Mathf.RoundToInt(verticalInput), 0);
                    if (TilemapBattleView.instance.ReachableCells.ContainsKey(position))
                    {
                        DoPath(position, TilemapBattleView.instance.ReachableCells[position]);
                    }
                    //_animator.SetTrigger(verticalInput > 0 ? "Back" : "Front");
                    return;
                }

                float horizontalInput = Input.GetAxisRaw("Horizontal");

                if (Mathf.Abs(horizontalInput) == 1)
                {
                    position += new Vector3Int(Mathf.RoundToInt(horizontalInput), 0, 0);
                    if (TilemapBattleView.instance.ReachableCells.ContainsKey(position))
                    {
                        DoPath(position, TilemapBattleView.instance.ReachableCells[position]);
                    }
                    //_animator.SetTrigger(horizontalInput > 0 ? "Right" : "Left");
                    return;
                }
            }
        }

        private void UnitStateChanged(Unit.State state)
        {
            switch (state)
            {
                case Unit.State.Idle:
                    _targeted = false;
                    break;
                case Unit.State.Active:
                    _targeted = false;
                    break;
                case Unit.State.Target:
                    if (CameraFollow.instance != null)
                    {
                        CameraFollow.instance.Target = gameObject;
                    }
                    _targeted = true;
                    TilemapBattleView.instance.ShowInfo(TeamIndex, _unit.Name, _unit.CurrentHealthPoints, _unit.TotalHealthPoints);
                    break;
                case Unit.State.Dead:
                    _spriteRenderer.enabled = false;
                    break;
            }
        }

        private void OnMoveUnit()
        {
            if (BattleController.instance.CurrentUnit == _unit)
            {
                if (!_unit.Team.IsAIControlled)
                {
                    _controlUnit = true;
                    _lastCell = _unit.Cell;
                    _cooldownDeltaTime = _inputCooldown;
                }

                if (CameraFollow.instance != null)
                {
                    CameraFollow.instance.Target = gameObject;
                }

                TilemapBattleView.instance.ShowInfo(TeamIndex, _unit.Name, _unit.CurrentHealthPoints, _unit.TotalHealthPoints);

                _animator.SetTrigger("Front");
            }
            else
            {
                _animator.SetTrigger("Idle");
            }
        }
    }
}