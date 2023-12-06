using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConstList
{
    /// <summary>
    /// プレイヤーがオブジェクトとどの向きで衝突したか.
    /// </summary>
    public enum HitDirList
    {
        NONE,
        HIT_RIGHT,
        HIT_UP,
        HIT_LEFT,
        HIT_DOWN,
    }

    /// <summary>
    /// プレイヤーの行動リスト.
    /// </summary>
    public enum PlayerAction
    {
        NONE,
        MOVE,
        JUMP,
        WALLJUMP,
        DANCE,
    }
}