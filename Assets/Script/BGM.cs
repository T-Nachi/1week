using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private void Awake()
    {
        // 同じBGMPlayerが複数存在しないようにする（シングルトン）
        if (FindObjectsOfType<BGM>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されない
    }
}
