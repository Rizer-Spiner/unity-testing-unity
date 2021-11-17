using System;
using System.Collections.Generic;
using System.Linq;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.Common
{
    public class Utils
    {
        public class StringUtils
        {
            public static string GetTimeString()
            {
                return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            }

            public static string GetRandomString(int length)
            {
                System.Random random = new System.Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            public static string GetMp4FileName(string name)
            {
                return GetTimeString() + (name == null ? "" : "-") + name + ".mp4";
            }

            public static string GetH264FileName(string name)
            {
                return GetTimeString() + (name == null ? "" : "-") + name + ".h264";
            }

            public static string GetWavFileName(string name)
            {
                return GetTimeString() + (name == null ? "" : "-") + name + ".wav";
            }

            public static string GetPngFileName(string name)
            {
                return GetTimeString() + (name == null ? "" : "-") + name + ".png";
            }

            public static string GetJpgFileName(string name)
            {
                return GetTimeString() + (name == null ? "" : "-") + name + ".jpg";
            }

            public static bool IsRtmpAddress(string str)
            {
                return (str != null && str.StartsWith("rtmp"));
            }
        }

        public class MathUtils
        {
            public static bool CheckPowerOfTwo(int input)
            {
                return (input & (input - 1)) == 0;
            }

            public static TimeInterval CreateTimeInterval(int secondsBeforeBug, float currentTime)
            {
                if (secondsBeforeBug >= currentTime)
                    return new TimeInterval()
                    {
                        start = 0f,
                        end = currentTime
                    };
                else if (secondsBeforeBug <= 0)
                    return new TimeInterval()
                    {
                        start = currentTime - 10f,
                        end = currentTime
                    };
                else return new TimeInterval()
                {
                    start = currentTime - secondsBeforeBug,
                    end = currentTime
                };

            }
        }

        public class JSONUtils
        {
            public static Dictionary<string, string> ConvertToDictionary(string json)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                json = json.Remove(json.Length - 1).Remove(0, 1);

                string[] pairs = json.Split(',');

                foreach (var pair in pairs)
                {
                    string[] keyValue = pair.Split(':');
                    string key = keyValue[0].Remove(keyValue[0].Length - 1).Remove(0, 1);
                    string value = keyValue[1].Remove(keyValue[1].Length - 1).Remove(0, 1);

                    dict.Add(key, value);
                }

                return dict;
            }
        }
    }
}