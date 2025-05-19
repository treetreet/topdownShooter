    using UnityEngine;

    public class CapturePointCtrl : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public int scorePerSecond = 10;
        private bool RedplayerInside = false;
        private bool BlueplayerInside = false;
        private float score = 0f;
        private float debugTimer = 0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.tag);
            if (other.CompareTag("RedPlayer"))
            {
                Debug.Log("Red 입장");
        
                RedplayerInside = true;
            }

            if (other.CompareTag("BluePlayer"))
            {
                Debug.Log("Blue 입장");
                BlueplayerInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("RedPlayer"))
            {
                Debug.Log("Red 퇴장");
                RedplayerInside = false;
            }

            if (other.CompareTag("BluePlayer"))
            {
                Debug.Log("Blue 퇴장");
                BlueplayerInside = false;
            }
        }

        private void Update()
        {
            if (BlueplayerInside && RedplayerInside)
            {
                   
            }
            else if (BlueplayerInside && !RedplayerInside)
            {
                score += scorePerSecond * Time.deltaTime;
            }
            else if (RedplayerInside && !BlueplayerInside)
            {
                score += -scorePerSecond * Time.deltaTime;
            }
            debugTimer += Time.deltaTime;

            if (debugTimer >= 1f)
            {
                Debug.Log($"점령 점수: {score:F2}");
                debugTimer = 0f;
            }
            
            if (score >= 100)
            {
                Debug.Log("BlueTeam Win!");
            }
            else if (score <= -100)
            {
                Debug.Log("RedTeam Win!");
            }
        }
    }
