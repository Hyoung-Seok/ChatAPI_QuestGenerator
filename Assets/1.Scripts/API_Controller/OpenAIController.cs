using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Models;
using UnityEngine;

public enum EModel
{
    GPT4,
    GPT4_TERBO,
    GPT4_Vision
}

public class OpenAIController : MonoBehaviour
{
    [Header("GPT Model")] 
    [SerializeField] private EModel gptModel;
    
    protected OpenAIAPI Api = null;
    protected Model Model = null;
    
    /// <summary>
    /// API 키와 모델 초기화
    /// </summary>
    protected virtual void InitGpt()
    {
        Api = new OpenAIAPI(KeyEncryption.DecodingBase64());
        SelectModel();
        
        Debug.Log("API Init Successful");
    }

    private void SelectModel()
    {
        switch (gptModel)
        {
            case EModel.GPT4:
                Model = Model.GPT4;
                break;
            
            case EModel.GPT4_TERBO:
                Model = Model.GPT4_Turbo;
                break;
            
            case EModel.GPT4_Vision:
                Model = Model.GPT4_Vision;
                break;
            
            default:
                return;
        }
    }
    
}
