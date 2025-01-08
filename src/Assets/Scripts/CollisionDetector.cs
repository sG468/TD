using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    //��`���m�̓����蔻��
    public static bool CheckRectCollision(Vector2 posA, Vector2 sizeA, Vector2 posB, Vector2 sizeB)
    {
        return posA.x < (posB.x + sizeB.x) && (posA.x + sizeA.x) > posB.x;
    }

    //���_�Ƃ̓����蔻��
    public static bool CheckBaseCollision(Vector2 charaPosition, Vector2 basePosition, string baseName)
    {
        if (baseName == "Enemy")
        {
            return basePosition.x >= charaPosition.x;
        }
        else
        {
            return basePosition.x <= charaPosition.x;
        }
    }
}
