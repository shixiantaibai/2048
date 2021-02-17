using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    private Image Bg;
    private Text number_text;
    private MyGrid inGrid;
    private float spawnScaleTime = 1;
    private bool isPlayingSpawnAnim = false;

    private float mergeSceleTime = 1;
    private float mergeSceleTimeBack = 1;
    private bool isPlayingMergeAnim = false;

    private float movePosTime = 1;
    private bool isMoving = false;
    private bool isDestroyOnMoveEnd = false;

    private Vector3 startMove, endMovePos;

    public NumberStatue status;
    public Color[] bg_colors;
    public List<int> number_list;
    public AudioClip mergeClip;

    private void Awake()
    {
        Bg = transform.GetComponent<Image>();
        number_text = transform.Find("Text").GetComponent<Text>();
    }
    public void Init(MyGrid m) {
        m.SetNumber(this);
        this.SetGrid(m);
        this.SetNumber(512);
        status = NumberStatue.Normal;
        PlaySpawnAnim();
    }
    public void SetGrid(MyGrid m) {
        this.inGrid = m;
    }
    public MyGrid GetGrid() {
        return inGrid;
    }
    public void SetNumber(int number) {
        this.number_text.text = number.ToString();
        this.Bg.color = this.bg_colors[number_list.IndexOf(number)];
    }
    public int GetNumber() {
        return int.Parse(number_text.text);
    }
    //传过来的参数是要移动到的哪个格子下
    public void MoveToGrid(MyGrid mg) {
        transform.SetParent(mg.transform);
        startMove = transform.localPosition;
        endMovePos = mg.transform.position;
        movePosTime = 0;
        isMoving = true;
        this.GetGrid().SetNumber(null);
        mg.SetNumber(this);
        this.SetGrid(mg);
    }
    // 在移动结束时候销毁
    public void DestroyOnMoveEnd(MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);
        startMove = transform.localPosition;

        movePosTime = 0;
        isMoving = true;
        isDestroyOnMoveEnd = true;
    }
    //合并数字的方法
    public void Merge() {

        GamePanel gamePanel = GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>();
        gamePanel.AddScore(this.GetNumber());
        int number = this.GetNumber() * 2;
        this.SetNumber(number);
        if (number == 2048)
        {
            // 游戏胜利了
            gamePanel.GameWin();
        }
        status = NumberStatue.NotMerge;
        PlayMergeAnim();
        // 播放音效
        AudioManager._instance.PlaySound(mergeClip);
    }

    public bool isMerge(Number n)
    {
        if (this.GetNumber()==n.GetNumber()&&n.status==NumberStatue.Normal) {
            return true;
        }
        return false;
    }

    // 播放创建动画
    public void PlaySpawnAnim()
    {
        spawnScaleTime = 0;
        isPlayingSpawnAnim = true;
    }
    public void PlayMergeAnim()
    {
        mergeSceleTime = 0;
        mergeSceleTimeBack = 0;
        isPlayingMergeAnim = true;
    }

    private void Update()
    {
        // 创建动画

        if (isPlayingSpawnAnim)
        {
            if (spawnScaleTime <= 1)
            {
                spawnScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, spawnScaleTime);
            }
            else
            {
                isPlayingSpawnAnim = false;
            }
        }


        // 合并动画

        if (isPlayingMergeAnim)
        {
            if (mergeSceleTime <= 1 && mergeSceleTimeBack == 0) // 变大的过程
            {
                mergeSceleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, mergeSceleTime);
            }
            if (mergeSceleTime >= 1 && mergeSceleTimeBack <= 1)
            {
                mergeSceleTimeBack += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, mergeSceleTimeBack);
            }

            if (mergeSceleTime >= 1 && mergeSceleTimeBack >= 1)
            {
                isPlayingMergeAnim = false;
            }
        }


        // 移动动画
        if (isMoving)
        {
            movePosTime += Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(startMove, Vector3.zero, movePosTime);
            // Debug.Log(" movePosTime: " + movePosTime + " pos: " + Vector3.Lerp(startMove, Vector3.zero, movePosTime));
            if (movePosTime >= 1)
            {
                isMoving = false;
                if (isDestroyOnMoveEnd)
                {
                   GameObject.Destroy(gameObject);
                    //.Instance.RecoverObject(gameObject);
                }

            }
        }

    }


    

}
