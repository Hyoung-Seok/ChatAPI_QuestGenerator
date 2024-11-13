using OpenAI_API.Chat;

public enum EToken
{
    Prompt,
    Completion,
    Total
}

// GPT4_Terbo 기준으로 가격 책정
// Input : $10.00 당 100000토큰
// Output : $30.00 당 100000토큰
public static class TokensData
{
    private static decimal _inputCost = 10m / 1000000m;
    private static decimal _outputCost = 30m / 1000000m;
    
    private static int[] _tokenData = new int[3];
    private static decimal[] _costData = new decimal[3];
    private static float _createTime = 0.0f;

    public static int[] TokenData => _tokenData;
    public static decimal[] CostData => _costData;
    public static float CreateTime => _createTime;
    
    public static void SaveTokenData(ChatUsage usage)
    {
        _tokenData[(int)EToken.Prompt] = usage.PromptTokens;
        _tokenData[(int)EToken.Completion] = usage.CompletionTokens;
        _tokenData[(int)EToken.Total] = usage.TotalTokens;
        
        ConvertTokenToDollar();
    }

    private static void ConvertTokenToDollar()
    {
        _costData[(int)EToken.Prompt] = _tokenData[(int)EToken.Prompt] * _inputCost;
        _costData[(int)EToken.Completion] = _tokenData[(int)EToken.Completion]  * _outputCost;
        _costData[(int)EToken.Total] = _costData[(int)EToken.Prompt] + _costData[(int)EToken.Completion];
    }

    public static void SaveCreateTime(float time)
    {
        _createTime = time;
    }
}
