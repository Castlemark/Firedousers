﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    void Initialize(int state);
    void ExecuteBehaviour();
    bool CanPass();
    bool IsFlammable();
    void SetSprite(int room_tileset);
}