using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Levels : MonoBehaviour
    
{
    private const float PIPE_WIDTH = 10f;
    private const float PIPE_HEAD_HEIGHT = 4f;
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;
    private const float BIRD_X_POSITION = 0f;



    private static Levels instance;

    public static Levels GetInstance()
    {
        return instance;
    }



    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipeSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
        }


    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,

    }

    //.............................................................................
    private void Awake()
    {
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1.5f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
        instance = this;

    }

    //.............................................................................
    private void Start()
    {
        Birds.GetInstance().OnDied += Birds_OnDied;
        Birds.GetInstance().OnStartedPlaying += Birds_OnStartedPlaying;

   
    }

    private void Birds_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Birds_OnDied(object sender, System.EventArgs e)
    {

        // CMDebug.TextPopupMouse("Dead!");
        state = State.BirdDead;
    /*
       FunctionTimer.Create(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Flappy 1");
        }, 1f);

    */

    }

   

    //.............................................................................
    private void Update ()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
       

    }
    //................................................................................
    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if( pipeSpawnTimer<0)
        {
            //time to spawn another pipe...................
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f+ heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;

            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height,gapSize, PIPE_SPAWN_X_POSITION);


        }
    }


    //.............................................................................
    private void HandlePipeMovement()
    {
        for (int i =0;i<pipeList.Count;i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
       
            pipe.Move();
            if(isToTheRightOfBird && pipe.GetXPosition()<=BIRD_X_POSITION && pipe.IsBottom())
            {
                //when pipe passes the bird
                pipesPassedCount++;
            }


            if(pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)                    // destroying pipes already passed the bird to free up memory .............
            {
                pipe.DestroySelf();
            pipeList.Remove(pipe);
            i--;
            }
        }
    }

    //.............................................................................


        private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                break;
            case Difficulty.Impossible:
                gapSize = 20f;
                break;

        }
    }
    //.................................................................................

        private Difficulty GetDifficulty()
    {
        if (pipeSpawned >=25) return Difficulty.Impossible;
        if (pipeSpawned >= 15) return Difficulty.Hard;
        if (pipeSpawned >= 5) return Difficulty.Medium;
        return Difficulty.Easy;
    }



    //.............................................................................
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipeSpawned++;
        SetDifficulty(GetDifficulty());


    }

    //.............................................................................
    private void CreatePipe(float height, float xPosition, bool createBottom )
    {
        // pipe head setting 

        Transform pipeHead = Instantiate(Game_Assets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * 0.5f;
        }
        else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * 0.5f;
        }

        pipeHead.position = new Vector3(xPosition,pipeHeadYPosition);
     
        //setting body (pipebody)
        Transform pipeBody = Instantiate(Game_Assets.GetInstance().pfPipeBody);

        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE ;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE ;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);
  
        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);


        Pipe pipe = new Pipe(pipeHead, pipeBody,createBottom);
        pipeList.Add(pipe);




    }


    public int GetPipesSpawned()
    {
        return pipeSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;

    }

    //.......................................................................................
    // >>>>>>>>>>>>>>we create this class for single pipe ,,,and above , we generate multiple pipes with reference to this >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool createBottom;


        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform,bool createBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.createBottom = createBottom;
        }
        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

        }
        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;

        }

        public bool IsBottom()
        {
            return createBottom; 
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}