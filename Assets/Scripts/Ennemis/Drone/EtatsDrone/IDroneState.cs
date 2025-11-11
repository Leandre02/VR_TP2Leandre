using UnityEngine;

/// <summary>
/// Represente un état du drone
/// </summary>
public interface IDroneState
{
    void EntrerEtat();
    void Update();
}
