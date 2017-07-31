using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class PrefsFileHandler <T> where T : new()
{
    private T obj = new T();
    private string _filePath = "";
    public void SetFilePath(string path)
    {
        _filePath = path;
    }



    public void GetFloat(string name, float def)
    {

    }
    public void SetFloat(string name, float set)
    {
        
    }
}