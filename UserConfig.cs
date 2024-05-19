using System.IO;
using System.Text.Json;

public class UserConfig
{
    public float CpuThreshold { get; set; }
    public float MemoryThreshold { get; set; }

    public static UserConfig LoadConfig(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<UserConfig>(json);
            return config ?? new UserConfig { CpuThreshold = 80, MemoryThreshold = 500 }; 
        }
        else
        {
            return new UserConfig { CpuThreshold = 80, MemoryThreshold = 500 };
        }
    }

    
    public void SaveConfig(string filePath)
    {
        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, json);
    }
}
