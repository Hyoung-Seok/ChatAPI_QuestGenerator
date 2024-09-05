using System;
using System.IO;
using System.Text;
using UnityEngine;
using File = System.IO.File;

public static class Base64
{
    private static string FILE_PATH = Application.streamingAssetsPath + "/API_KEY.txt";
    
    public static bool EncodingBase64(string str)
    {
        var strByte = Encoding.UTF8.GetBytes(str);

        try
        {
            File.WriteAllText(FILE_PATH, Convert.ToBase64String(strByte));
            return true;
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public static string DecodingBase64()
    {
        var key = File.ReadAllText(FILE_PATH);
        var strByte = Convert.FromBase64String(key);

        return Encoding.UTF8.GetString(strByte);
    }
}
