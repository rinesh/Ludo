using System.Collections.Generic;

public class GameManager
{
    public const int MAX_BLOCKS_GLOBAL = 51;
    const int MAX_BLOCKS = 56;
    const int BASE_OFFSET = 13;
    const int HOME_POS = -1;
    const int DICE_MAX = 6;

    class Player
    {
        int m_id = -1;
        int m_rank = -1;
        
        int[] tokens = new int[4] { HOME_POS, HOME_POS, HOME_POS, HOME_POS };

        public void SetPlayerId(int pos_)
        {
            m_id = pos_;
        }

        public void SetRank(int rank_)
        {
            m_rank = rank_;
        }

        public int GetRank()
        {
            return m_rank;
        }

        public void SetTokenRelativePosition(int index_, int pos_)
        {
            tokens[index_] = pos_;
        }

        public int GetTokenRelativePosition(int index_)
        {
            return tokens[index_];
        }

        public int GetTokenGlobalPosition(int index_)
        {
            return (tokens[index_] + (BASE_OFFSET * m_id)) % (MAX_BLOCKS_GLOBAL+1);
        }

        public bool AllTokensReachedGoal()
        {
            for (int i = 0; i < 4; i++)
            {
                if (tokens[i] < MAX_BLOCKS)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
    List<int> m_safeBlocks = new List<int> { 0, 8, 13, 21, 26, 34, 39, 47 };
    Player[] m_players = new Player[4];
    int m_totalPlayers = 4;
    int m_currentPlayer = 0;
    int m_diceValue = -1;
    int m_curretnRank = 0;

    static GameManager m_instance = null;

    private GameManager()
    {

    }

    public static GameManager GetInstance()
    {
        if (m_instance == null)
            m_instance = new GameManager();

        return m_instance;
    }

    public void InitPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            m_players[i] = new Player();
            m_players[i].SetPlayerId(i);
        }
    }

    public int GetCurrentPlayer()
    {
        return m_currentPlayer;
    }

    public int GetTokenRelativePosition(int player_, int token_)
    {
        return m_players[player_].GetTokenRelativePosition(token_);
    }

    public int GetTokenGlobalPosition(int player_, int token_)
    {
        return m_players[player_].GetTokenGlobalPosition(token_);
    }

    public (int, List<int>, bool) PlayTurn()
    {
        m_diceValue = UnityEngine.Random.Range(1, 7);

        (List<int> movableTokens, bool turnChanged) = GetMovableTokens();

        return (m_diceValue, movableTokens, turnChanged);
    }

    public (List<int>, bool) GetMovableTokens()
    {
        List<int> movableTokens = new List<int>();
        bool turnchanged = false;

        for (int i = 0; i < 4; i++)
        {
            int tokenPos = m_players[m_currentPlayer].GetTokenRelativePosition(i);
            if ((tokenPos == HOME_POS && m_diceValue == 6) || (tokenPos > HOME_POS && tokenPos + m_diceValue <= MAX_BLOCKS))
            {
                movableTokens.Add(i);
            }
        }

        //check turn change
        if (movableTokens.Count<=0 && m_diceValue < DICE_MAX)
        {
            do
            {
                m_currentPlayer += 1;
                m_currentPlayer %= 4;
            }
            while (m_players[m_currentPlayer].GetRank() > 0);

            turnchanged = true;
        }

        return (movableTokens, turnchanged);
    }

    public (List<(int, int)>, bool, int) MoveToken(int index_)
    {
        bool turnchanged = false;

        int pos = m_players[m_currentPlayer].GetTokenRelativePosition(index_);
        if (pos == HOME_POS)
        {
            pos = 0;
        }
        else
        {
            pos += m_diceValue;
        }

        m_players[m_currentPlayer].SetTokenRelativePosition(index_, pos);

        int globalPos = m_players[m_currentPlayer].GetTokenGlobalPosition(index_);

        List<(int, int)> tokensCaptured = new List<(int, int)>();

        if (pos < MAX_BLOCKS_GLOBAL && !m_safeBlocks.Contains(globalPos))
        {
            tokensCaptured = CaptureOpponentTokens(globalPos); 
        }

        if (m_players[m_currentPlayer].AllTokensReachedGoal())
        {
            m_curretnRank++;
            m_players[m_currentPlayer].SetRank(m_curretnRank);
        }

        //check turn change
        if (m_diceValue != DICE_MAX && tokensCaptured.Count<=0)
        {
            do
            {
                m_currentPlayer += 1;
                m_currentPlayer %= 4;
            }
            while (m_players[m_currentPlayer].GetRank() > 0);

            turnchanged = true;
        }

        return (tokensCaptured, turnchanged, m_curretnRank);
    }

    List<(int, int)> CaptureOpponentTokens(int globalPos_)
    {
        List<(int, int)> tokensKilled = new List<(int, int)>();

        for (int i = 0; i < 4; i++)
        {
            if (i == m_currentPlayer)
                continue;

            for (int j = 0; j < 4; j++)
            {
                int p = m_players[i].GetTokenRelativePosition(j);
                if (p > 0 && p <= MAX_BLOCKS_GLOBAL)
                {
                    int gp = m_players[i].GetTokenGlobalPosition(j);
                    if (gp == globalPos_)
                    {
                        m_players[i].SetTokenRelativePosition(j, HOME_POS);
                        tokensKilled.Add((i, j));
                    }
                }
            }
        }

        return tokensKilled;
    }

    bool IsGameOver()
    {
        if (m_curretnRank >= m_totalPlayers-1)
            return true;

        return false;
    }
}
