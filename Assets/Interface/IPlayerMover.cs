using UnityEngine;
public enum MoveLockType
{
    None,
    HorizontalOnly,
    FullLock
}
public interface IPlayerMover
{
    public void SetMoveLock(MoveLockType lockType);
}