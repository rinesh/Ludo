using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    enum STATE
    {
        DICE_ROLL,
        MOVE,
    }

    STATE m_state = STATE.DICE_ROLL;
    public List<Player> m_players;
    public Text m_text;
    public GameObject[] m_pathObjs;

    int m_prevPlayer;

    GameManager m_gameManager = null;

    public static GameBoard Instance = null;

    private void Awake()
    {
        Instance = this;

        m_gameManager = GameManager.GetInstance();
    }

    //Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            m_players[i].SetTokenIds();
        }

        m_prevPlayer = m_gameManager.GetCurrentPlayer();
        ChangeCurrentPlayer();
    }

    public void RollDice(UnityEngine.UI.Text text_)
    {
        if (m_state != STATE.DICE_ROLL)
            return;

        m_prevPlayer = m_gameManager.GetCurrentPlayer();

        (int diceValue, List<int> movableTokens, bool turnChanged) = m_gameManager.PlayTurn();
        text_.text = diceValue.ToString();

        int currentPlayer = m_gameManager.GetCurrentPlayer();

        if (turnChanged)
        {
            m_state = STATE.DICE_ROLL;
            ChangeCurrentPlayer();
        }
        else if(movableTokens.Count>0)
        {
            m_players[currentPlayer].SetMovableTokens(movableTokens);
            m_state = STATE.MOVE;
        }
    }

    void ChangeCurrentPlayer()
    {
        m_players[m_prevPlayer].SetHighlight(false);
        m_players[m_prevPlayer].SetState(Player.STATE.WAIT);

        int currentPlayer = m_gameManager.GetCurrentPlayer();
        m_players[currentPlayer].SetHighlight(true);
        m_players[currentPlayer].SetState(Player.STATE.TURN);
    }

    public void MoveToken(int tokenId_)
    {
        int currentPlayer = m_gameManager.GetCurrentPlayer();

        (List<(int player, int token)> tokensCaptured, bool turnChanged, int rank) = m_gameManager.MoveToken(tokenId_);

        MaptoBoard(currentPlayer, tokenId_);

        for(int i = 0;i<tokensCaptured.Count;i++)
        {
            int player = tokensCaptured[i].player;
            int token = tokensCaptured[i].token;

            MaptoBoard(player, token);
        }
        
        if (turnChanged)
        {
            ChangeCurrentPlayer();
        }
        else
        {
            m_players[currentPlayer].SetState(Player.STATE.TURN);
        }

        m_players[currentPlayer].ResetTokens();
        m_text.text = "ROLL";
        m_state = STATE.DICE_ROLL;
    }

    void MaptoBoard(int player_, int tokenId_)
    {
        int lPos = m_gameManager.GetTokenRelativePosition(player_, tokenId_);
        int gPos = m_gameManager.GetTokenGlobalPosition(player_, tokenId_);

        if(lPos<0)
        {
            m_players[player_].SetHomePosition(tokenId_);
        }
        else if (lPos>= 0 && lPos < GameManager.MAX_BLOCKS_GLOBAL)
        {
            m_players[player_].SetTransform(tokenId_, m_pathObjs[gPos].transform.position);
        }
        else if(lPos >= GameManager.MAX_BLOCKS_GLOBAL)
        {
            int offset = 6;

            int p = lPos + 1  + (player_ * offset);
            
            m_players[player_].SetTransform(tokenId_, m_pathObjs[p].transform.position);
        }
    }
}
