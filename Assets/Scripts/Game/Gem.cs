using UnityEngine;

namespace Game
{
    public class Gem : MonoBehaviour
    {
        public GameObject player1;
        public GameObject player2;
        public GameObject collectSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player1)
            {
                Destroy();
                GameManager.GemOwner = player1;
            }

            if (other.gameObject == player2)
            {
                Destroy();
                GameManager.GemOwner = player2;
            }
        }

        private void Destroy()
        {
            Instantiate(collectSound, Vector3.zero, Quaternion.identity).SetActive(true);
            Destroy(gameObject);
        }
    }
}