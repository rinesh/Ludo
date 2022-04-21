using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Animator m_highlightAnimator;
	public Coin[] m_tokens;

	public enum STATE
	{
		WAIT,
		TURN,
		MOVE,
	};

	STATE m_state = STATE.WAIT;

	public void SetState(STATE state_)
    {
		m_state = state_;
    }

	public void SetTokenIds()
	{
		for (int i = 0; i < 4; i++)
		{
			m_tokens[i].SetId(i);
		}
	}

	public void SetHighlight(bool enabled_)
    {
		m_highlightAnimator.SetBool("highlight", enabled_);
	}

	public void SetMovableTokens(List<int> movable_)
    {
		m_state = STATE.MOVE;

		foreach(int t in movable_)
        {
			m_tokens[t].SetMovable(true);
		}
    }

	public void ResetTokens()
	{
		for (int i = 0; i < 4; i++)
		{
			m_tokens[i].SetMovable(false);
		}
	}

	public void SetTransform(int tokenId_, Vector3 pos_)
	{
		m_tokens[tokenId_].transform.position = pos_;
	}

	public void SetHomePosition(int tokenId_)
	{
		for (int i = 0; i < 4; i++)
		{
			m_tokens[tokenId_].SetHomePosition();
		}
	}
}
