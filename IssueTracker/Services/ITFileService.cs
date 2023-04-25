using IssueTracker.Services.Interfaces;

namespace IssueTracker.Services;

public class ITFileService : IITFileService
{
    #region Properties
    private readonly string[] suffixes = { "bytes", "KB", "MB", "GB", "TB", "PB" };
    #endregion
    
    #region Convert File To Byte Array 
    public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
    {
        try
        {
            MemoryStream memoryStream = new();
            await file.CopyToAsync(memoryStream);
            byte[] byteFile = memoryStream.ToArray();
            
            // Garbage collection. Don't have to do but better to be explicit
            memoryStream.Close();
            memoryStream.Dispose();

            return byteFile;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error converting file to byte array. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Convert Byte Array To File
    public string ConvertByteArrayToFile(byte[] fileData, string extension)
    {
        try
        {
            string imageBase64Data = Convert.ToBase64String(fileData);
            return string.Format($"data:{extension};base64,{imageBase64Data}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error converting byte array to file. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get File Icon
    public string GetFileIcon(string file)
    {
        string ext = Path.GetExtension(file).Replace(".", "");
        return $"/img/contenttype/{ext}.png";
    }
    #endregion
    
    #region Format File Size
    public string FormatFileSize(long bytes)
    {
        int counter = 0;
        decimal fileSize = bytes;
        while (Math.Round(fileSize / 1024) >= 1)
        {
            fileSize /= bytes;
            counter++;
        }

        return string.Format("{0:n1}{1}", fileSize, suffixes[counter]);
    }
    #endregion
}