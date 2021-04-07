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

public static class Formula
{
    public static int EnemyHpFormula(EnemyModel model)
    {
        int minHp = model.MinHp;
        int maxHp = model.MaxHp;
        float intense = model.HpIntense;
        float diff = (maxHp - minHp) * 0.5f;
        float average = (minHp + maxHp) * 0.5f;

        int Hp = Random.Range(minHp, maxHp + 1);
        
        if(intense == 0.5f)
        {
            
        }
        else
        {
            if(intense > 0.5f)
            {
                if(intense == 1.0f)
                {
                    return maxHp;
                }
                else
                {
                    float f = (intense - 0.5f) * 2f;
                    
                    if(average + f * diff > Hp && Random.Range(0, 1f) <= intense)
                    {
                        return Random.Range(minHp, maxHp + 1);
                    }
                }
            }
            else
            {
                if(intense == 0f)
                {
                    return minHp;
                }
                else
                {
                    float f = (0.5f - intense) * 2f;
                    
                    if(average - f * diff < Hp && Random.Range(0, 1f) >= intense)
                    {
                        return Random.Range(minHp, maxHp + 1);
                    }
                }
            }
        }

        return Hp;
    }
}