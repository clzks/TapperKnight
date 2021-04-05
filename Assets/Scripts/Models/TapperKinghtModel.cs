using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapperKinghtModel : MonoBehaviour, IModel
{
    // 스테이지 별 몬스터 등장 데이터를 들고있어야 하며
    // 프레젠터에게 요청받을 시 해당 몬스터의 정보를 뷰에게 넘겨준다.
    // 뷰는 몬스터의 정보를 받아서 생성 및 세팅한다.

    private List<StageModel> stageModelList;

    private void Awake()
    {
        SetSampleStageModel();    
    }

    public void SetSampleStageModel()
    {
        stageModelList = new List<StageModel>();

        for(int i = 0; i < 8; ++i)
        {
            StageModel sm = new StageModel();

            sm.Id = i;
            sm.StageNumber = i + 1;
            sm.MinimumGenCycle = 3;
            sm.MaximumGenCycle = 5;
            sm.GenDegreeRatio = 0.5f;
            sm.TotalTime = 50f;
            stageModelList.Add(sm);
        }
    }

    public void SetStageModel()
    {

    }

    public List<StageModel> GetStageModelList()
    {
        return stageModelList;
    }
    
    public StageModel GetStageModel(int index)
    {
        return stageModelList[index];
    }
}
