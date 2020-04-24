using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    AudioSource audioPlayer;

    public AudioClip scoreSound, jumpSound, deadSound;

    public bl_Joystick joystick;

    float movHorizontal, movVertical;
    public float velocidad = 1.0f;
    public float altitud = 100.0f;
    public float totalTime = 120f;

    public bool isJumping = false;
    bool paused = false;

    int stars = 0; int lives = 3;
    
    public Text starsText, livesText, timeText, finalStarsText, finalLivesText;

    public GameObject startPoint, panelGameOver, panelCongrats, uiMobile;

    public MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    { 
        #if UNITY_ANDROID
            uiMobile.SetActive(true);
        #endif

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!paused){
            CountDown();
        }

        //Obtener inputs del teclado
        movVertical = Input.GetAxis("Vertical");
        movHorizontal = Input.GetAxis("Horizontal");

        #if UNITY_ANDROID
            movVertical = joystick.Vertical * 0.12f;
            movHorizontal = joystick.Horizontal * 0.12f;
        #endif

        Vector3 movimiento = new Vector3(movHorizontal, 0.0f, movVertical);

        rb.AddForce(movimiento * velocidad);

        if(Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Floor" || collision.gameObject.name == "Wood"){
            isJumping = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "Star"){
            Destroy(collider.gameObject);
            stars += 1;
            starsText.text = "0" + stars.ToString();

            GetComponent<AudioSource>().clip = scoreSound;
            GetComponent<AudioSource>().Play();
            
        }

        if(collider.gameObject.name == "DeadZone" || collider.gameObject.name == "Axe"){
            transform.position = startPoint.transform.position;
            lives -= 1;
            livesText.text = "0" + lives.ToString();
            if(lives == 0){
                GameOver();
            }

            GetComponent<AudioSource>().clip = deadSound;
            GetComponent<AudioSource>().Play();
        }

        if(collider.gameObject.name == "Final"){
           FinishedGame();
        }
    }

    void CountDown(){
        totalTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime - (minutes * 60));

        timeText.text = string.Format("{0:0}:{01:00}", minutes, seconds);

        if(minutes == 0 && seconds == 0) {
            GameOver();
        }
    }

    public void PauseGame(){
        paused = !paused;
        rb.isKinematic = paused;
    }

    void GameOver(){
        menuManager.GoToMenu(panelGameOver);
        PauseGame();
    }

    public void RestartGame(){
        totalTime = 120f;
        lives = 3;
        stars = 0;
        livesText.text = "03";
        starsText.text = "00";
        paused = false;
        transform.position = startPoint.transform.position;
        rb.isKinematic = true;

    }

    public void FinishedGame(){
        PauseGame();
        menuManager.GoToMenu(panelCongrats);
        finalLivesText.text = "0" + lives.ToString();
        finalStarsText.text = "0" + stars.ToString();
    }

    public void Jump(){
        if(!isJumping){
            Vector3 salto = new Vector3(0, altitud, 0);
            rb.AddForce(salto * velocidad);
            isJumping = true;

            GetComponent<AudioSource>().clip = jumpSound;
            GetComponent<AudioSource>().Play();
        }
    }
}
