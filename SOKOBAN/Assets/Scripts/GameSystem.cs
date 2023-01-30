// ---------------------------------------------------------  
// GameSystem.cs  
//   
// 作成日:  2022/10/23
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [SerializeField] MapGeneration MapGene;
    [Tooltip("マップ配列")] private int[,] MapArray;

    [SerializeField,Tooltip("プレイヤーi座標")] private int PlayerPos_i;
    [SerializeField,Tooltip("プレイヤーj座標")] private int PlayerPos_j;

    #region プレイヤーモデル
    //プレイヤーモデル
    enum PLAYER_MODEL
    {
        UP_MODEL,
        DOWN_MODEL,
        LEFT_MODEL,
        RIGHT_MODEL,
    }

    [AddArrayName(new string[] { "前モデル", "後ろモデル", "左モデル", "右モデル" })]
    [SerializeField] private GameObject[] models=new GameObject[4];

    #endregion


    [AddArrayName(new string[] { "前", "後", "左", "右" })]
    [SerializeField] private bool[] _isPlayerMoveOK = new bool[4];

    [AddArrayName(new string[] { "前", "後", "左", "右" })]
    [SerializeField] private bool[] _isBoxMoveOK = new bool[4];


    //移動しようとしているマス
    private int UpPlan;
    private int DownPlan;
    private int LeftPlan;
    private int RightPlan;

    //２マス先のマス
    private int UpScheme;
    private int DownScheme;
    private int LeftScheme;
    private int RightScheme;

    //動くべき箱
    [SerializeField] private GameObject UpMoveBox;
    [SerializeField] private GameObject DownMoveBox;
    [SerializeField] private GameObject LeftMoveBox;
    [SerializeField] private GameObject RightMoveBox;

    //すべての箱
    [SerializeField]private GameObject[] Boxes;

    //すべてのゴール
    private GameObject[] Goals;
    private int ClearIndex=0;//クリア状態の箱の個数

    //Audio周り
    private AudioSource SEAudioSource;
    [SerializeField] private AudioClip SE_Move;
    [SerializeField] private AudioClip SE_Clear;
    enum OBJ_TYPE
    {
        /// <summary>
        /// 地面
        /// </summary>
        GROUND,
        /// <summary>
        /// 壁
        /// </summary>
        WALL,
        /// <summary>
        /// プレイヤー
        /// </summary>
        PLAYER,
        /// <summary>
        /// 箱
        /// </summary>
        BOX,
        /// <summary>
        /// ゴール
        /// </summary>
        GOAL,
    }

    [SerializeField] public bool isGameClear=false;

    [SerializeField] private Image PerfectUI;

    #region レイヤー
    private int PlayerLayer = -4;
    private int BoxLayer = -6;
    #endregion

    private void Start()
    {
        MapGene = GameObject.FindWithTag("SOKOBAN").GetComponent<MapGeneration>();

        MapArray = MapGene.GetMapArray();//マップ配列を格納
        PlayerPos_i = MapGene.PlayerPos[0];//プレイヤーのi座標を格納
        PlayerPos_j = MapGene.PlayerPos[1];//プレイヤーのj座標を格納

        Boxes = GameObject.FindGameObjectsWithTag("Box");//全箱の配列
        Goals = GameObject.FindGameObjectsWithTag("Goal");//全ゴールの配列

        ClearIndex = 0;

        SEAudioSource = GetComponent<AudioSource>();
    } 



    private void Update()
    {
        if (!isGameClear)
        {
            PlanUpdate();
            CheckMoveOK();
            PlayerMovement();
            StageClear();
        }

        SceneLoad();
    }


    /// <summary>
    /// プレイヤーを中心とした１マス周囲や２マス周囲の取得
    /// </summary>
    private void PlanUpdate()
    {
        //プレイヤーを中心とした周囲４マスの取得
        UpPlan = MapArray[PlayerPos_i - 1, PlayerPos_j];
        DownPlan = MapArray[PlayerPos_i + 1, PlayerPos_j];
        LeftPlan = MapArray[PlayerPos_i, PlayerPos_j - 1];
        RightPlan = MapArray[PlayerPos_i, PlayerPos_j + 1];

        //プレイヤーを中心として２マス離れた周囲４マスの取得
        UpScheme = MapArray[PlayerPos_i - 2, PlayerPos_j];
        DownScheme = MapArray[PlayerPos_i + 2, PlayerPos_j];
        LeftScheme = MapArray[PlayerPos_i, PlayerPos_j - 2];
        RightScheme = MapArray[PlayerPos_i, PlayerPos_j + 2];
    }



    /// <summary>
    /// 動いてもいいのかを確認する
    /// </summary>
    private void CheckMoveOK()
    {
        //プレイヤーの前マスの探索
        if (UpPlan == (int)OBJ_TYPE.GROUND || 
            UpPlan == (int)OBJ_TYPE.PLAYER || 
            UpPlan == (int)OBJ_TYPE.GOAL)//プレイヤーの前が地面か地面かゴールの場合、（一応バグ消しでプレイヤーも）
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.UP_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.UP_MODEL] = false;//箱移動の不許可
        }
        else if (UpPlan == (int)OBJ_TYPE.BOX && 
            UpScheme != (int)OBJ_TYPE.WALL && 
            UpScheme != (int)OBJ_TYPE.BOX)//プレイヤーの前が箱でその先が壁か箱でない時、
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.UP_MODEL] = true;//プレイヤーの移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.UP_MODEL] = true;//箱移動の許可

            for (int i = 0; i < Boxes.Length; i++)//指定された箱の探索
            {
                if (Boxes[i].gameObject.transform.position == new Vector3(PlayerPos_j, -(PlayerPos_i - 1), BoxLayer))
                {
                    UpMoveBox = Boxes[i];
                    break;
                }
            }
        }
        else
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.UP_MODEL] = false;//プレイヤーの移動の不許可
            _isBoxMoveOK[(int)PLAYER_MODEL.UP_MODEL] = false;//箱移動の不許可
        }

        //プレイヤーの後マスの探索
        if (DownPlan == (int)OBJ_TYPE.GROUND ||
            DownPlan == (int)OBJ_TYPE.PLAYER ||
            DownPlan == (int)OBJ_TYPE.GOAL)//プレイヤーの後が地面か地面かゴールの場合、（一応バグ消しでプレイヤーも）
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = false;//箱移動の不許可
        }
        else if (DownPlan == (int)OBJ_TYPE.BOX &&
            DownScheme != (int)OBJ_TYPE.WALL &&
            DownScheme != (int)OBJ_TYPE.BOX)//プレイヤーの後が箱でその先が壁か箱でない時、
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = true;//箱移動の許可

            for (int i = 0; i < Boxes.Length; i++)//指定された箱の探索
            {
                if (Boxes[i].gameObject.transform.position == new Vector3(PlayerPos_j, -(PlayerPos_i + 1), BoxLayer))
                {
                    DownMoveBox = Boxes[i];
                    break;
                }
            }
        }
        else
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = false;//プレイヤーの移動の不許可
            _isBoxMoveOK[(int)PLAYER_MODEL.DOWN_MODEL] = false;//箱移動の不許可
        }

        //プレイヤーの左マスの探索
        if (LeftPlan == (int)OBJ_TYPE.GROUND ||
            LeftPlan == (int)OBJ_TYPE.PLAYER ||
            LeftPlan == (int)OBJ_TYPE.GOAL)//プレイヤーの左が地面か地面かゴールの場合、（一応バグ消しでプレイヤーも）
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = false;//箱移動の不許可
        }
        else if (LeftPlan == (int)OBJ_TYPE.BOX &&
            LeftScheme != (int)OBJ_TYPE.WALL && 
            LeftScheme != (int)OBJ_TYPE.BOX)//プレイヤーの左が箱でその先が壁か箱でない時、
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = true;//箱移動の許可

            for (int i = 0; i < Boxes.Length; i++)//指定された箱の探索
            {
                if (Boxes[i].gameObject.transform.position == new Vector3(PlayerPos_j-1, -PlayerPos_i, BoxLayer))
                {
                    LeftMoveBox = Boxes[i];
                    break;
                }
            }
        }
        else
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = false;//プレイヤーの移動の不許可
            _isBoxMoveOK[(int)PLAYER_MODEL.LEFT_MODEL] = false;//箱移動の不許可
        }

        //プレイヤーの右マスの探索
        if (RightPlan == (int)OBJ_TYPE.GROUND ||
            RightPlan == (int)OBJ_TYPE.PLAYER ||
            RightPlan == (int)OBJ_TYPE.GOAL)//プレイヤーの右が地面か地面かゴールの場合、（一応バグ消しでプレイヤーも）
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = false;//箱移動の不許可
        }
        else if (RightPlan == (int)OBJ_TYPE.BOX &&
            RightScheme != (int)OBJ_TYPE.WALL &&
            RightScheme != (int)OBJ_TYPE.BOX)//プレイヤーの左が箱でその先が壁か箱でない時、
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = true;//プレイヤー移動の許可
            _isBoxMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = true;//箱移動の許可

            for (int i = 0; i < Boxes.Length; i++)//指定された箱の探索
            {
                if (Boxes[i].gameObject.transform.position == new Vector3(PlayerPos_j + 1, -PlayerPos_i, BoxLayer))
                {
                    RightMoveBox = Boxes[i];
                    break;
                }
            }
        }
        else
        {
            _isPlayerMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = false;//プレイヤーの移動の不許可
            _isBoxMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL] = false;//箱移動の不許可
        }
    }



    
    /// <summary>
    /// プレイヤーの移動メソッド
    /// </summary>
    private void PlayerMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))//上キーを押したら
        {
            ModelActiveFalse();
            models[(int)PLAYER_MODEL.UP_MODEL].SetActive(true);//上に行くモデルを出す

            if (_isPlayerMoveOK[(int)PLAYER_MODEL.UP_MODEL])//移動ができるなら
            {
                PlayerPos_i--;//プレイヤーの座標を移動させて。
                PlayerPosReload();//更新する。 
                try
                {
                    SEAudioSource.PlayOneShot(SE_Move);
                }
                catch { }

            }

            if (_isBoxMoveOK[(int)PLAYER_MODEL.UP_MODEL])
            {
                UpMoveBox.gameObject.transform.position = new Vector3(PlayerPos_j, -(PlayerPos_i - 1), BoxLayer);//箱を移動
                MapArray[PlayerPos_i-1, PlayerPos_j] = (int)OBJ_TYPE.BOX;//配列を入れ替え
                MapArray[PlayerPos_i, PlayerPos_j] = (int)OBJ_TYPE.GROUND;//配列を入れ替え
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))//下キーを押したら
        {
            ModelActiveFalse();
            models[(int)PLAYER_MODEL.DOWN_MODEL].SetActive(true);//下に行くモデルを出す

            if (_isPlayerMoveOK[(int)PLAYER_MODEL.DOWN_MODEL])//移動ができるなら
            {
                PlayerPos_i++;//プレイヤーの座標を移動させて。
                PlayerPosReload();//更新する。
                try
                {
                    SEAudioSource.PlayOneShot(SE_Move);
                }
                catch { }
            }

            if (_isBoxMoveOK[(int)PLAYER_MODEL.DOWN_MODEL])
            {
                DownMoveBox.gameObject.transform.position = new Vector3(PlayerPos_j, -(PlayerPos_i + 1), BoxLayer);//箱を移動
                MapArray[PlayerPos_i + 1, PlayerPos_j] = (int)OBJ_TYPE.BOX;//配列を入れ替え
                MapArray[PlayerPos_i, PlayerPos_j] = (int)OBJ_TYPE.GROUND;//配列を入れ替え
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))//左キーを押したら
        {
            ModelActiveFalse();
            models[(int)PLAYER_MODEL.LEFT_MODEL].SetActive(true);//左に行くモデルを出す

            if (_isPlayerMoveOK[(int)PLAYER_MODEL.LEFT_MODEL])//移動ができるなら
            {
                PlayerPos_j--;//プレイヤーの座標を移動させて
                PlayerPosReload();//更新する。
                try
                {
                    SEAudioSource.PlayOneShot(SE_Move);
                }
                catch { }
            }

            if (_isBoxMoveOK[(int)PLAYER_MODEL.LEFT_MODEL])
            {
                LeftMoveBox.gameObject.transform.position = new Vector3(PlayerPos_j-1, -PlayerPos_i, BoxLayer);//箱を移動
                MapArray[PlayerPos_i, PlayerPos_j-1] = (int)OBJ_TYPE.BOX;//配列を入れ替え
                MapArray[PlayerPos_i, PlayerPos_j] = (int)OBJ_TYPE.GROUND;//配列を入れ替え
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))//右キーに押したら
        {
            ModelActiveFalse();
            models[(int)PLAYER_MODEL.RIGHT_MODEL].SetActive(true);//右に行くモデルを出す

            if (_isPlayerMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL])//移動ができるなら
            {
                //Stackin();
                PlayerPos_j++;//プレイヤーの座標を移動させて
                PlayerPosReload() ;//更新する。
                try
                {
                    SEAudioSource.PlayOneShot(SE_Move);
                }
                catch { }
            }

            if (_isBoxMoveOK[(int)PLAYER_MODEL.RIGHT_MODEL])
            {
                RightMoveBox.gameObject.transform.position = new Vector3(PlayerPos_j + 1, -PlayerPos_i, BoxLayer);//箱を移動
                MapArray[PlayerPos_i, PlayerPos_j+1] = (int)OBJ_TYPE.BOX;//配列を入れ替え
                MapArray[PlayerPos_i, PlayerPos_j] = (int)OBJ_TYPE.GROUND;//配列を入れ替え
            }
        }
    }



    /// <summary>
    /// モデルをすべてfalseにする。
    /// </summary>
    private void ModelActiveFalse()
    {
        for (int i=0; i<models.Length; i++)
        {
            models[i].SetActive(false);
        }
    }


    /// <summary>
    /// プレイヤーの位置を更新するメソッド
    /// </summary>
    private void PlayerPosReload()
    {
        this.transform.position = new Vector3(PlayerPos_j, -PlayerPos_i, PlayerLayer) ;
    }


    /// <summary>
    /// 箱を規定個規定ポイントに運んだらクリアっていうのを取るメソッド
    /// </summary>
    private void StageClear()
    {
        for(int i = 0; i < Goals.Length; i++)//ゴールチェック
        {
            for(int j=0; j < Boxes.Length; j++)
            {
                if (Goals[i].transform.position.x == Boxes[j].transform.position.x&&
                    Goals[i].transform.position.y==Boxes[j].transform.position.y)
                {
                    ClearIndex++;
                }
            }

        }

        if (ClearIndex >= Goals.Length)//ゴールしている数とゴールの個数が一致した時
        {
            Debug.Log("Game Clear");
            isGameClear = true;
            SEAudioSource.PlayOneShot(SE_Clear);
            MapGeneration.Stage++;
        }
        else//リセット
        {
            ClearIndex = 0;
        }
    }


    /// <summary>
    /// リスタートやタイトルに遷移等
    /// </summary>
    private void SceneLoad()
    {
        if (Input.GetKeyDown(KeyCode.Return))//Enterキーでリスタート
        {
            SceneManager.LoadScene("SOKOBAN");
        }

        if (Input.GetKeyDown(KeyCode.Escape))//エスケープでタイトルへ戻る
        {
            SceneManager.LoadScene("Title");
        }
    }
}
