using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GhostSheepBehavior : AgentBehaviour
{
    private Random random = new();
    private DateTime creationTime = DateTime.Now;
    public GameObject gameManager;
    public GameObject player1;
    public GameObject player2;
    public GameObject centerCollider;
    private float _timeToWait = 0;

    public bool _isGhost = false;

    public void Start()
    {
    }


    public override void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        base.FixedUpdate();
        var robot = agent._celluloRobot;

        if (_isGhost)
            agent.SetVisualEffect(VisualEffect.VisualEffectBlink, Color.red, 125);
        else if (!_isGhost)
            agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.blue, 0);
        if (_isGhost && random.NextDouble() < dt / 7f) // the ghost-sheep is in average ghost for 7seconds  
            _isGhost = false;
        else if (!_isGhost && random.NextDouble() < dt / 15f) // the ghost-sheep is in average sheep for 15seconds
            _isGhost = true;

        _timeToWait -= dt;
        
    }

    public override Steering GetSteering()
    {
        var steering = new Steering();
        //implement your code here.

        var selfPos = transform.position;
        var p1Pos = player1.transform.position;
        var p2Pos = player2.transform.position;
        Vector3 direction = Vector3.zero;
        float distance1 = Vector3.Distance(selfPos, p1Pos),
            distance2 = Vector3.Distance(selfPos, p2Pos);
        if (_isGhost)
        {
            var nearestPos = distance1 > distance2 ? p2Pos : p1Pos;
            direction += (nearestPos - selfPos);
        }
        else
        {
            direction += (selfPos - p1Pos) / (distance1 * distance1);
            direction += (selfPos - p2Pos) / (distance2 * distance2);
        }

        direction = direction.normalized;
        var manager = gameManager.GetComponent<GameManager>();
        if (!manager.hasStarted || manager.IsGameFinished())
            direction = Vector3.zero;
        if (_timeToWait > 0 && _isGhost)
            direction = Vector3.zero;
        steering.linear = new Vector3(direction.x, 0, direction.z) * agent.maxAccel;
        steering.linear =
            transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
        return steering;
    }

    private void wait(float seconds)
    {
        _timeToWait = seconds;
    }

    private bool isWaiting()
    {
        return _timeToWait > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        var manager = gameManager.GetComponent<GameManager>();
        var o = other.gameObject;
        if (isWaiting()) return;
        if ((o == player1 || o == player2) && _isGhost)
        {
            manager.OnGhostTouchPlayer(o);
            wait(2);
        }
        else if (o == centerCollider && !_isGhost)
        {
            manager.OnSheepInCenter();
            wait(3.5f);
        }
    }
}