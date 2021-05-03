using UnityEngine;

public enum NoteType
{
    Left,
    Right,
    BothSide,
    Null,
    Count
}

public enum ScoreType
{
    Miss,
    Bad,
    Good,
    Great,
    Perfect,
    Count
}

public enum InGameState
{ 
    Ready,
    Play,
    Change,
    Count
}
public enum ObjectType
{ 
    Enemy,
    Note,
    Effect,
    Score,
    Count
}

public enum CharacterAnimType
{ 
    Idle,
    Walk,
    Run,
    Attack,
    Attack2,
    Skill,
    Dead,
    Count
}

public static class Formula
{
    public static Vector3 BezierMove(Vector3 start, Vector3 p, Vector3 dest, float t)
    {
        Vector3 first = Vector3.Lerp(start, p, t);
        Vector3 second = Vector3.Lerp(p, dest, t);

        return Vector3.Lerp(first, second, t);
    }
}