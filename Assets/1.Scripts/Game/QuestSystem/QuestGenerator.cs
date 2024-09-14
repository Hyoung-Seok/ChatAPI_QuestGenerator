using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private NpcDataDisplay npcData;
    [SerializeField] private API_JsonMod jsonAPI;

    [Header("Path")] 
    [SerializeField] private string filePath;
    
    private ExcelWriter _excelWriter;
    private string _systemMessage;

    private void Start()
    {
        _excelWriter = new ExcelWriter(Application.dataPath + filePath);
        
        InitSystemMessage();
    }

    public void CreateJsonMessage()
    {
        jsonAPI.CreateJsonMessage(npcData.NpcData);
    }
    
    private void InitSystemMessage()
    {
        _systemMessage = "너는 게임의 퀘스트를 생성하는 어시스던트야." +
                         "기본적인 정보를 입력받아 퀘스트를 생성할 수 있도록 JSON형식으로 출력하도록 해." +
                         "기본적인 Key값은 다음과 같아." +
                         "QuestID : 퀘스트의 아이디, Title : 퀘스트의 제목, " +
                         "Type : 퀘스트의 타입. 이 타입에는 Fight(전투), Get(얻어오기), Deliver(전달하기)가 있어. 이 퀘스트 타입은 사용자가 알려줄거야." +
                         "NpcName : 이 퀘스트를 주는 NPC의 이름." +
                         "Target : 잡아야 할 몬스터, 얻어야 할 아이템, 전달 목표인 npc이름이 들어가." +
                         "Count : 갯수" +
                         "Scripts : 퀘스트를 줄 때 NPC의 대사. 대사는 플레이어가 퀘스트를 받을 때 나타나는 대사와 퀘스트를 수락하고 나타나는 대사를 만들어줘." +
                         "이 두 종류의 대사를 구분하기 위해 대사 사이에는 *을 넣어줘";
        
        jsonAPI.InitSystemMessage(_systemMessage);
    }
}
