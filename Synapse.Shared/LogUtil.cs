using Synapse.Shared.Models;
using System.Text.Json;

namespace Synapse.Shared
{
    public static class LogUtil
    {
        public static List<ApplicationTime> GetApplicationsTime(string filePath)
        {
            if (File.Exists(filePath))
            {
                var isReaded = false;
                while (!isReaded)
                {
                    try
                    {
                        var lines = File.ReadAllLines(filePath).ToList();
                        if (lines.Any())
                        {
                            return ConvertToApplicationsTimeModel(lines);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        isReaded = true;
                    }
                }
            }
            return new List<ApplicationTime>();
        }

        public static List<DeviceTime> GetDevicesTime(string filePath)
        {
            if (File.Exists(filePath))
            {
                var isReaded = false;
                while (!isReaded)
                {
                    try
                    {
                        var lines = File.ReadAllLines(filePath).ToList();
                        if (lines.Any())
                        {
                            return ConvertToDevicesTimeModel(lines);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        isReaded = true;
                    }
                }
            }
            return new List<DeviceTime>();
        }

        public static List<DownloadLogDto> GetDownloadHistory(string filePath)
        {
            var downloadHistoryJson = new List<DownloadLogDto>();
            if (File.Exists(filePath))
            {
                var isReaded = false;
                while (!isReaded)
                {
                    try
                    {
                        string jsonString = File.ReadAllText(filePath);
                        var dataFromFile = JsonSerializer.Deserialize<List<DownloadLogDto>>(jsonString);
                        if (dataFromFile != null && dataFromFile.Any())
                        {
                            downloadHistoryJson.AddRange(dataFromFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        isReaded = true;
                    }
                }
            }
            return downloadHistoryJson;
        }

        private static List<DeviceTime> ConvertToDevicesTimeModel(List<string> lines)
        {
            var lstmodel = new List<DeviceTime>();
            foreach (var line in lines)
            {
                string[] cells = line.Split('\t');
                lstmodel.Add(
                    new DeviceTime()
                    {
                        TimeStart = DateTime.Parse(cells[0]),
                        TrackingTime = long.Parse(cells[1]),
                        TimeStamp = DateTime.Parse(cells[2]),
                    }
                );
            }
            return lstmodel;
        }

        private static List<ApplicationTime> ConvertToApplicationsTimeModel(List<string> lines)
        {
            var lstmodel = new List<ApplicationTime>();
            foreach (var line in lines)
            {
                string[] cells = line.Split('\t');

                var model = new ApplicationTime
                {
                    ProcessName = cells[0],
                    TrackingTime = long.Parse(cells[1]),
                    ApplicationName = cells[2],
                    AppVersion = cells[3],
                    TimeUtcLog = cells.Length > 4 ? DateTime.Parse(cells[4]) : DateTime.UtcNow,
                };
                lstmodel.Add(model);
            }
            return lstmodel;
        }

        public static void CleanTimeLog(string filePath)
        {
            var isCleaned = false;
            while (!isCleaned)
            {
                try
                {
                    File.WriteAllLines(filePath, new List<string>());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    isCleaned = true;
                }
            }
        }

        public static void CleanDownloadHistoryLog(string filePath)
        {
            var isCleaned = false;
            while (!isCleaned)
            {
                try
                {
                    var downloadLogJson = JsonSerializer.Serialize(new List<DownloadLogDto>());
                    File.WriteAllText(filePath, downloadLogJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    isCleaned = true;
                }
            }
        }

    }
}
