using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
*  作   者 ：胡朋
*  github : https://github.com/xiaomoinfo
*  描   述 ：修炼场
*/
public class Training : MonoBehaviour
{
    public float getSkillTime = 10f;

    private bool startTrain;

    public Text trainText;

    private void Start()
    {
        trainText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (startTrain)
        {
            if (getSkillTime > 0)
            {
                getSkillTime -= Time.deltaTime;
            }
            else
            {
                Debug.LogWarning("修炼成功");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        startTrain = true;
        trainText.gameObject.SetActive(true);
        trainText.text = "修炼神功中...:" + getSkillTime;
    }

    private void OnTriggerExit(Collider other)
    {
        trainText.text = "中断修炼";
        startTrain = false;
        getSkillTime = 10f;
        trainText.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
    }
}