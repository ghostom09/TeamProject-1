using UnityEngine;

public interface IPullable
{
    public void PullTo(Vector2 pos, float force);
}