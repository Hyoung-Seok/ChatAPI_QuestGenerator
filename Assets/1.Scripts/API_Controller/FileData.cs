public class FileData
{
    private string _fileID;
    private string _fileName;
    private string _filePurpose;
    private long _fileDate;
    private string _fileStatus;

    public string ID => _fileID;
    public string Name => _fileName;
    public string Purpose => _filePurpose;
    public long Date => _fileDate;
    public string Status => _fileStatus;

    public FileData(string id, string name, string purpose, long date, string status)
    {
        _fileID = id;
        _fileName = name;
        _filePurpose = purpose;
        _fileDate = date;
        _fileStatus = status;
    }
    
}
