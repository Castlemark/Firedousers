using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudPeople : MonoBehaviour
{
    public Sprite[] sprites;

    Image m_Image;

    void Start()
    {
        m_Image = GetComponent<Image>();
    }

    public void ChangeSprite(int state)
    {
        if (state == -1)
        {
            m_Image.enabled = false;
        }
        else
        {
            m_Image.enabled = true;
            m_Image.sprite = sprites[state];
        }

    }

}
