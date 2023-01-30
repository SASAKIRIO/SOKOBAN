// ---------------------------------------------------------  
// MapGeneration.cs  
//   
// 作成日:  
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [Tooltip("マップの縦（４以上であること）")] public int Map_i = 9;
    [Tooltip("マップの横（４以上であること）")] public int Map_j = 10;

    [Tooltip("ゴールの座標位置")] public int[,] GoalPos=new int[10,2];

    [AddArrayName(new string[] { "移動可能ブロック", "壁", "プレイヤー", "箱", "ゴール" })]
    [SerializeField, Tooltip("プレハブ")] private GameObject[] prefabs;

    /*
     * マップ２次元配列
     * 
     * 0:移動可能ポジション
     * 1:移動不可ポジション
     * 2:プレイヤーポジション
     * 3:箱ポジション
     * 4:ゴールポジション
     * 5:箱とゴールが同じ場所にいるとき
     */

    enum OBJ_TYPE
    {
        GROUND,
        WALL,
        PLAYER,
        BOX,
        GOAL,
    }

    private int[,] MAP = new int[9, 10];//ランダムマップ(縦、横)=(i,j)
    private int[,] MAP_1st =new int[9,10]//リセットマップ
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,4,2,3,0,4,1,1 },
        {1,1,1,0,3,3,3,4,1,1 },
        {1,1,1,4,0,3,0,4,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_2nd = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,4,4,0,1,1,1,1,1 },
        {1,1,0,0,0,0,1,1,1,1 },
        {1,1,1,0,0,0,0,1,1,1 },
        {1,1,1,1,0,3,3,0,1,1 },
        {1,1,1,1,1,0,0,2,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_3rd = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,0,0,0,1,1,1,1,1 },
        {1,1,0,0,3,0,1,1,1,1 },
        {1,1,0,3,2,3,0,1,1,1 },
        {1,1,0,0,3,3,0,1,1,1 },
        {1,1,4,4,4,4,4,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_4th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,2,0,0,0,0,0,1,1 },
        {1,1,0,1,0,0,1,0,1,1 },
        {1,1,0,0,0,0,0,0,1,1 },
        {1,1,1,1,1,1,0,0,1,1 },
        {1,1,0,3,0,0,3,0,1,1 },
        {1,1,0,0,4,4,0,0,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_5th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,2,4,0,4,0,0,1,1 },
        {1,1,0,1,0,0,0,0,1,1 },
        {1,1,0,0,0,1,1,1,1,1 },
        {1,1,0,3,0,1,1,1,1,1 },
        {1,1,0,3,0,1,1,1,1,1 },
        {1,1,0,0,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_6th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,2,4,1,1,1 },
        {1,1,1,1,1,3,3,1,1,1 },
        {1,1,1,1,1,0,0,1,1,1 },
        {1,1,1,1,1,3,0,1,1,1 },
        {1,1,1,1,4,0,0,1,1,1 },
        {1,1,1,1,4,0,0,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_7th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,2,0,1,1,1,1 },
        {1,1,1,1,0,3,0,1,1,1 },
        {1,1,1,1,4,3,4,1,1,1 },
        {1,1,1,1,4,3,0,1,1,1 },
        {1,1,1,1,0,0,0,1,1,1 },
        {1,1,1,1,0,0,0,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_8th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,0,0,1,1 },
        {1,1,1,1,1,1,3,0,1,1 },
        {1,1,2,0,0,1,0,4,1,1 },
        {1,1,0,0,3,0,0,0,1,1 },
        {1,1,1,1,1,0,0,4,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_9th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,2,0,4,1,1 },
        {1,1,1,1,1,0,3,0,1,1 },
        {1,1,1,1,1,1,0,1,1,1 },
        {1,1,0,0,1,0,0,4,1,1 },
        {1,1,0,3,3,3,0,0,1,1 },
        {1,1,0,0,0,0,4,4,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };

    private int[,] MAP_10th = new int[9, 10]
    {
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,2,0,0,4,0,4,1,1 },
        {1,1,0,3,3,0,3,0,1,1 },
        {1,1,1,0,1,1,3,1,1,1 },
        {1,1,0,0,1,4,0,0,1,1 },
        {1,1,0,4,1,0,0,0,1,1 },
        {1,1,1,1,1,1,0,0,1,1 },
        {1,1,1,1,1,1,1,1,1,1 },
        {1,1,1,1,1,1,1,1,1,1 }
    };


    [SerializeField] public static int Stage;

    [AddArrayName(new string[] {"プレイヤーのi座標","プレイヤーのj座標"})]
    public int[] PlayerPos = new int[2];//PlayerPosのi座標j座標の格納



    //以下メソッドーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー



    private void Start()
    {
        MapSet();

        CreateMap();
    }

    private void MapSet()
    {
        switch (Stage)
        {
            case 1:
                MAP = MAP_1st;
                break;
            case 2:
                MAP = MAP_2nd;
                break;
            case 3:
                MAP = MAP_3rd;
                break;
            case 4:
                MAP = MAP_4th;
                break;
            case 5:
                MAP = MAP_5th;
                break;
            case 6:
                MAP = MAP_6th;
                break;
            case 7:
                MAP = MAP_7th;
                break;
            case 8:
                MAP = MAP_8th;
                break;
            case 9:
                MAP = MAP_9th;
                break;
            case 10:
                MAP = MAP_10th;
                break;
            default:
                Stage = 1;
                MAP = MAP_1st;
                break;
        }
    }


    public int[,] GetMapArray()
    {
        return MAP;
    }



    private void CreateMap()
    {
        int loc_BoxIndex = 0;
        int loc_GoalIndex = 0;


        //各マスの情報を格納
        for (int j = 0; j < Map_j; j++)
        {
            for(int i = 0; i < Map_i; i++)
            {
                if (MAP[i,j]==(int)OBJ_TYPE.PLAYER)//マスがプレイヤーなら
                {
                    PlayerPos[0] = i;
                    PlayerPos[1] = j;
                }
                else if (MAP[i, j] == (int)OBJ_TYPE.BOX)//マスが箱なら
                {
                    loc_BoxIndex++;
                }
                else if (MAP[i, j] == (int)OBJ_TYPE.GOAL)//マスがゴールなら
                {
                    GoalPos[loc_GoalIndex,0] = i;//ゴールのi座標を格納
                    GoalPos[loc_GoalIndex,1] = j;//ゴールのj座標を格納

                    loc_GoalIndex++;
                }


                OBJ_TYPE objType = (OBJ_TYPE)(MAP[i, j]);//数字じゃ分かりにくいからenum変数に書き換え

                if (MAP[i, j] != (int)OBJ_TYPE.GROUND && MAP[i,j]!=(int)OBJ_TYPE.WALL)//地面を貼る（地面２重にならないようにするif文)
                {
                    GameObject PlayerGround = Instantiate(prefabs[0]);
                    PlayerGround.transform.position = new Vector2(j, -i);
                }


                GameObject tileObj=Instantiate(prefabs[(int)objType]);//配列MAPのインデックス(i,j)に対応するオブジェクトをプレハブから取る。


                if (MAP[i, j] == (int)OBJ_TYPE.BOX)
                {
                    tileObj.transform.position = new Vector3(j, -i, -6);//箱をレイヤー最優先
                }
                else if (MAP[i, j] == (int)OBJ_TYPE.PLAYER)
                {
                    tileObj.transform.position = new Vector3(j, -i, -4);//プレイヤーは次に優先
                }
                else
                {
                    tileObj.transform.position = new Vector2(j, -i);//配列の順番通りに配置
                }

                if (MAP[i, j] == (int)OBJ_TYPE.GOAL)
                {
                    tileObj.transform.position = new Vector3(j, -i, -2);//ゴールは次に優先
                }
            }
        }
    }
}
