using UnityEngine;

public class Coin : MonoBehaviour
{
	int m_id = -1;
	bool m_movable = false;
	Vector3 m_homePos;

	void Start()
	{
		m_homePos = transform.position;
	}

	void OnMouseDown()
	{
		if(m_movable)
		{
			GameBoard.Instance.MoveToken(m_id);
		}
	}

	public void SetMovable(bool value_)
    {
		m_movable = value_;
		GetComponent<Animator>().SetBool("highlight", value_);
		Vector3 pos = transform.position;
		pos.z = value_ ? -1.0f:0.0f;
		transform.position = pos;
	}

	public void SetId(int id_)
    {
		m_id = id_;
    }

	public void SetHomePosition()
    {
		transform.position = m_homePos;
	}
}
