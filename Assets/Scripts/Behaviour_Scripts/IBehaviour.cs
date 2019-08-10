using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    int state { get; set; }
    void Initialize(int state);
    void ExecuteBehaviour();
    bool CanPass();
    bool IsFlammable();
    void SetSprite(int room_tileset);
}
