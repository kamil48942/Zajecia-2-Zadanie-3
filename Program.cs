using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Text.Json;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]  // Dodanie atrybutu na poziomie klasy
class Program
{
    static System.Timers.Timer timer = new System.Timers.Timer(5000); // Inicjalizacja timera z interwałem 5 sekund
    static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    static UserConfig config = new UserConfig { CpuThreshold = 80, MemoryThreshold = 500 }; // Inicjalizacja domyślnej konfiguracji
    static string configFilePath = "user_config.json";

    static void Main(string[] args)
    {
        config = UserConfig.LoadConfig(configFilePath);

        // Konfiguracja timera (inicjalizacja już została wykonana przy deklaracji)
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;

        Console.WriteLine("Monitoring started. Press Enter to exit.");
        Console.ReadLine();

        config.SaveConfig(configFilePath);
    }

    private static void OnTimedEvent(object? source, ElapsedEventArgs e)  // Dopuszczenie wartości null dla parametru source
    {
        float cpuUsage = cpuCounter.NextValue();
        float availableMemory = ramCounter.NextValue();

        Console.WriteLine($"CPU Usage: {cpuUsage}%");
        Console.WriteLine($"Available Memory: {availableMemory} MB");

        if (cpuUsage > config.CpuThreshold)
        {
            LogEvent("High CPU usage detected: " + cpuUsage + "%");
        }

        if (availableMemory < config.MemoryThreshold)
        {
            LogEvent("Low available memory detected: " + availableMemory + " MB");
        }

        LogParameters(cpuUsage, availableMemory);
    }

    private static void LogEvent(string message)
    {
        if (!EventLog.SourceExists("SystemMonitoringTool"))
        {
            EventLog.CreateEventSource("SystemMonitoringTool", "Application");
        }

        EventLog.WriteEntry("SystemMonitoringTool", message, EventLogEntryType.Warning);
    }

    private static void LogParameters(float cpuUsage, float availableMemory)
    {
        string logFilePath = "system_monitor_log.txt";
        using (StreamWriter sw = new StreamWriter(logFilePath, true))
        {
            sw.WriteLine($"{DateTime.Now}: CPU Usage: {cpuUsage}%, Available Memory: {availableMemory} MB");
        }
    }
}
