using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ITarget
{
    public Sprite Icon { get; }
    IReadOnlyList<ClientTargetPoint> ClientTargetPoints { get; }
}