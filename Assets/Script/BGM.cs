using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private void Awake()
    {
        // ����BGMPlayer���������݂��Ȃ��悤�ɂ���i�V���O���g���j
        if (FindObjectsOfType<BGM>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // �V�[�����܂����ł��j������Ȃ�
    }
}
