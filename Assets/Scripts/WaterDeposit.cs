using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterDeposit : MonoBehaviour
{
    public Sprite[] images;
    Image m_Image;

    void Start()
    {
        //Fetch the Image from the GameObject
        m_Image = GetComponent<Image>();
    }

    public void ChangeSprite(int water)
    {
        if(water == 0)
        {
            m_Image.enabled = false;
        }
        else
        {
            m_Image.enabled = true;
            m_Image.sprite = images[5 - water];
        }

    }
}
