using Game;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Core.Behaviors
{
    public class GhostSheepBehavior : AgentBehaviour
    {
        private Random random = new();
        public GameObject gameManager;
        public GameObject player1;
        public GameObject player2;
        public GameObject centerCollider;
        private float _timeToWait = 0;

        public bool isGhost = false;
        
        public void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            var robots = new[]
            {
                player1.GetComponent<CelluloAgent>()._celluloRobot, player2.GetComponent<CelluloAgent>()._celluloRobot
            };
            foreach (var r in robots)
                r?.SetCasualBackdriveAssistEnabled(!isGhost);


            var robot = agent._celluloRobot;
            if (isGhost)
            {
                agent.SetVisualEffect(VisualEffect.VisualEffectBlink, Color.red, 125);
                robot?.SetVisualEffect((long)VisualEffect.VisualEffectBlink, 255, 0, 0, 125);
            }
            else
            {
                agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.blue, 0);
                robot?.SetVisualEffect((long)VisualEffect.VisualEffectConstAll, 255, 0, 0, 125);
            }

            if (isGhost && random.NextDouble() < dt / 7f) // the ghost-sheep is in average ghost for 7seconds  
                isGhost = false;
            else if (!isGhost && random.NextDouble() < dt / 15f) // the ghost-sheep is in average sheep for 15seconds
                isGhost = true;

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
            if (isGhost)
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
            if (_timeToWait > 0 && isGhost)
                direction = Vector3.zero;
            steering.linear = new Vector3(direction.x, 0, direction.z) * agent.maxAccel;
            steering.linear =
                transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
            return steering;
        }

        private void Wait(float seconds)
        {
            _timeToWait = seconds;
        }

        private bool IsWaiting()
        {
            return _timeToWait > 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            var manager = gameManager.GetComponent<GameManager>();
            var o = other.gameObject;
            if (IsWaiting()) return;
            if ((o == player1 || o == player2) && isGhost)
            {
                manager.OnGhostTouchPlayer(o);
                Wait(2);
            }
            else if (o == centerCollider && !isGhost)
            {
                manager.OnSheepInCenter();
                Wait(3.5f);
            }
        }
    }
}