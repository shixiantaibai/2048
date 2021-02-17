using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    #region UI控件
    public Text t_score;
    public Text t_bestscore;
    public Button b_last;
    public Button b_restart;
    public Button b_exit;
    public int currentScore;
    public WinPanel winPanel;
    public LosePanel losePanel;

    #endregion

    #region 属性 变量
    public GameObject gridPrefab;
    public GameObject numberPrefab;
    public Transform gridParent;
    public StepModel lastStepModel;
    public Dictionary<int, int> grid_config = new Dictionary<int, int>() { { 4,30},{ 5,26},{6,22 } };
    private int row;
    private int col;
    private Vector3 pointerUp, pointerDown;
    private bool isNeedCreatNumber=false;
    public MyGrid[][] grids = null;
    public List<MyGrid> canCreatNumberGrids=new List<MyGrid>();
    public AudioClip bgClip;
    #endregion

    #region 游戏初始化
    public void InitGrid() {
        //获取格子数量
        int gridNumber = PlayerPrefs.GetInt(Const.GameModel,4);
        GridLayoutGroup gridLayoutGroup = gridParent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraintCount = gridNumber;
        gridLayoutGroup.cellSize = new Vector2(grid_config[gridNumber], grid_config[gridNumber]);
        //创建格子
        grids = new MyGrid[gridNumber][];
        row = gridNumber;
        col = gridNumber;
        for (int i=0;i<row;i++) {
            for (int j=0;j<col;j++) {
                if (grids[i]==null) { grids[i] = new MyGrid[gridNumber]; }
                grids[i][j]=CreatGrid();
            }
        }
    }
    public MyGrid CreatGrid() {
        GameObject g= GameObject.Instantiate(gridPrefab,gridParent);
        return g.GetComponent<MyGrid>();
    }
    public void CreatNumbers() {
        canCreatNumberGrids.Clear();
        for (int i=0;i<row;i++) {
            for (int j=0;j<col;j++) {
                if (!grids[i][j].isHaveNum()) {
                    canCreatNumberGrids.Add(grids[i][j]);
                }
            }
        }
        if (canCreatNumberGrids.Count==0) { return; }
        int index = Random.Range(0,canCreatNumberGrids.Count);
        GameObject gobj=GameObject.Instantiate(numberPrefab,canCreatNumberGrids[index].transform);
        gobj.GetComponent<Number>().Init(canCreatNumberGrids[index]);
    }
    public void CreatNumbers(MyGrid myGrid, int number)
    {
        //GameObject gameObj = ObjectPool.Instance.LoadObject("Prefab/Number");
        //gameObj.transform.SetParent(myGrid.transform);
        //gameObj.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        //gameObj.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        GameObject gameObj = GameObject.Instantiate(numberPrefab, myGrid.transform);
        gameObj.GetComponent<Number>().Init(myGrid); // 进行初始化
        gameObj.GetComponent<Number>().SetNumber(number);
    }
    #endregion

    private void Awake()
    {
        // 初始化界面信息
        InitPanelMessage();
        InitGrid();
        CreatNumbers();
    }

    #region 监听事件
    // 上一步
    public void OnLastClick()
    {

        BackToLastStep();
        b_last.interactable = false;
    }
    public void OnReStartClick() {
        RestartGame();
    }
    public void OnExitGameClick() {
        ExitGame();
    }
    public void OnPointerDown() {
        pointerDown = Input.mousePosition;
    }
    public void OnPointerUp() {
        pointerUp = Input.mousePosition;
        if (Vector3.Distance(pointerDown,pointerUp)<30) {
            Debug.Log("无效操作");
            return;
        }
        // 保存数据
        lastStepModel.UpdateData(this.currentScore, PlayerPrefs.GetInt(Const.BestScore, 0), grids);
        b_last.interactable = true;

        MoveType mt = CaculateMoveType();
        Debug.Log("向"+ mt + "移动");
        MoveNumber(mt);
        if (isNeedCreatNumber) {
            CreatNumbers();
        }
        ResetNumberStatus();
        isNeedCreatNumber = false;
        if (IsGameLose())
        { // 说明游戏结束
            GameLose();
        }
    }
    #endregion

    //判断是左右还是上下移动
    public MoveType CaculateMoveType() {
        if (Mathf.Abs(pointerUp.x - pointerDown.x) > Mathf.Abs(pointerUp.y - pointerDown.y))
        {
            //左右移动
            if (pointerUp.x > pointerDown.x)
            {
                return MoveType.RIGHT;
            }
            else
            {
                return MoveType.LEFT;
            }
        }
        else
        {
            //上下移动
            if (pointerUp.y > pointerDown.y)
            {
                return MoveType.UP;
            }
            else
            {
                return MoveType.DOWN;
            }
        }
    }
    //判断移动方向
    public void MoveNumber(MoveType mt) {
        switch (mt)
        {
            case MoveType.UP:
                for (int j=0;j<col;j++) {
                    for (int i=1;i<row;i++) {
                        if (grids[i][j].isHaveNum()) {
                            Number number = grids[i][j].GetNumber();
                            for (int m=i-1;m>=0;m--) {
                                Number targetNumber = null;
                                if (grids[m][j].isHaveNum()) {
                                    targetNumber = grids[m][j].GetNumber();
                                }
                                HandleNumber(number,targetNumber, grids[m][j]);
                                if (targetNumber!=null) {
                                    break;
                                }
                            }
                        }
                    }
                }               

                break;
            case MoveType.DOWN:

                for (int j = 0; j < col; j++)
                {
                    for (int i = row-2; i>=0; i--)
                    {
                        if (grids[i][j].isHaveNum())
                        {
                            Number number = grids[i][j].GetNumber();
                            for (int m = i + 1; m <row; m++)
                            {
                                Number targetNumber = null;
                                if (grids[m][j].isHaveNum())
                                {
                                    targetNumber = grids[m][j].GetNumber();
                                }
                                HandleNumber(number, targetNumber, grids[m][j]);
                                if (targetNumber != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case MoveType.LEFT:

                for (int i=0;i<row;i++) {
                    for (int j=1;j<col;j++) {
                        if (grids[i][j].isHaveNum())
                        {
                            Number number = grids[i][j].GetNumber();
                            for (int m = j - 1; m >= 0; m--)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].isHaveNum())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }
                                HandleNumber(number, targetNumber, grids[i][m]);
                                if (targetNumber != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case MoveType.RIGHT:

                for (int i = 0; i < row; i++)
                {
                    for (int j = col-2; j >=0; j--)
                    {
                        if (grids[i][j].isHaveNum())
                        {
                            Number number = grids[i][j].GetNumber();
                            for (int m = j + 1; m<col; m++)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].isHaveNum())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }
                                HandleNumber(number, targetNumber, grids[i][m]);
                                if (targetNumber != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void HandleNumber(Number current,Number target,MyGrid targetGrid) {
        if (targetGrid.isHaveNum())
        {
            if (current.isMerge(target)) {
                target.Merge();
                current.GetGrid().SetNumber(null);//这句话不同
                //GameObject.Destroy(current.gameObject);
                current.DestroyOnMoveEnd(target.GetGrid());
                isNeedCreatNumber = true;
            }
        }
        else {
            current.MoveToGrid(targetGrid);
            isNeedCreatNumber = true;
        }
    }

    public void ResetNumberStatus() {
        for (int i=0;i<row;i++) {
            for (int j=0;j<col;j++) {
                if (grids[i][j].isHaveNum()) {
                    grids[i][j].GetNumber().status = NumberStatue.Normal;
                }
            }
        }
    }

    // 重新开始
    public void RestartGame()
    {
        // 数据清空 
        // 清空分数
        ResetScore();
        // 清空数字 
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i][j].isHaveNum()) {
                    GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                    grids[i][j].SetNumber(null);
                }
                
            }
        }
        // 创建一个数字 
        CreatNumbers();

    }
    //退出游戏
    public void ExitGame()
    {
        // 把对象池清空
        //SubPool subPool = ObjectPool.Instance.GetSubPool("Prefab/Number");
        //subPool.RecoverAllObject();

        SceneManager.LoadSceneAsync(0);
    }
    //初始化面板数据
    public void InitPanelMessage()
    {
        this.t_bestscore.text = PlayerPrefs.GetInt(Const.BestScore, 0).ToString();
        lastStepModel = new StepModel();
        b_last.interactable = false;

        // 播放音乐
        AudioManager._instance.PlayMusic(bgClip);

    }
    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScore(currentScore);

        // 判断当前的分数是不是最高的分数
        if (currentScore > PlayerPrefs.GetInt(Const.BestScore, 0))
        {
            PlayerPrefs.SetInt(Const.BestScore, currentScore);
            UpdateBestScore(currentScore);
        }

    }
    //重置分数
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScore(currentScore);
    }
    //更新分数
    public void UpdateScore(int score)
    {
        this.t_score.text = score.ToString();
    }
    //更新最好的分数
    public void UpdateBestScore(int bestScore)
    {
        this.t_bestscore.text = bestScore.ToString();
    }
    //上一步
    public void BackToLastStep()
    {
        // 分数
        currentScore = lastStepModel.score;
        UpdateScore(lastStepModel.score);

        PlayerPrefs.SetInt(Const.BestScore, lastStepModel.bestScore);
        UpdateBestScore(lastStepModel.bestScore);

        // 数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (lastStepModel.numbers[i][j] == 0)
                {
                    if (grids[i][j].isHaveNum())
                    {
                        //grids[i][j].GetNumber().Destroy();
                        GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                        grids[i][j].SetNumber(null);
                    }
                }
                else if (lastStepModel.numbers[i][j] != 0)
                {
                    if (grids[i][j].isHaveNum())
                    {
                        // 修改数字
                        grids[i][j].GetNumber().SetNumber(lastStepModel.numbers[i][j]);
                    }
                    else
                    {
                        // 创建数字
                        CreatNumbers(grids[i][j], lastStepModel.numbers[i][j]);
                    }
                }
            }
        }
    }
    public void GameWin()
    {
        Debug.Log("游戏胜利了");
        winPanel.Show();
    }
    public void GameLose()
    {
        Debug.Log("游戏失败了");
       losePanel.Show();
    }
    // 判断游戏是否失败
    public bool IsGameLose()
    {

        // 判断格子是否满了 
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!grids[i][j].isHaveNum())
                {
                    return false;
                }
            }
        }
        // 判断有没有数字能够合并
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {

                MyGrid up = IsHaveGrid(i - 1, j) ? grids[i - 1][j] : null;
                MyGrid down = IsHaveGrid(i + 1, j) ? grids[i + 1][j] : null;
                MyGrid left = IsHaveGrid(i, j - 1) ? grids[i][j - 1] : null;
                MyGrid right = IsHaveGrid(i, j + 1) ? grids[i][j + 1] : null;
                // grids[i][j] 
                if (up != null)
                {
                    if (grids[i][j].GetNumber().isMerge(up.GetNumber()))
                    {
                        return false;
                    }
                }

                if (down != null)
                {
                    if (grids[i][j].GetNumber().isMerge(down.GetNumber()))
                    {
                        return false;
                    }
                }

                if (left != null)
                {
                    if (grids[i][j].GetNumber().isMerge(left.GetNumber()))
                    {
                        return false;
                    }
                }

                if (right != null)
                {
                    if (grids[i][j].GetNumber().isMerge(right.GetNumber()))
                    {
                        return false;
                    }
                }


            }
        }


        return true;// 游戏失败
    }
    public bool IsHaveGrid(int i, int j)
    {

        if (i >= 0 && i < row && j >= 0 && j < col)
        {
            return true;
        }

        return false;
    }
}
