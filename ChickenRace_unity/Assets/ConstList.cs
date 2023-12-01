using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConstList
{
    /// <summary>
    /// �v���C���[���I�u�W�F�N�g�Ƃǂ̌����ŏՓ˂�����.
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
    /// �v���C���[�̍s�����X�g.
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