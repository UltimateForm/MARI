using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IGameController  {

	GameStates State { get; set; }

    void SetState(GameStates value);
    void AddState(GameStates value);
    void RemoveState(GameStates value);
}
