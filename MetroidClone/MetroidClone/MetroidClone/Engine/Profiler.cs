#if DEBUG
#define USING_PROFILER
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class Profiler
    {
        public bool LogToConsole { set; protected get; }
#if USING_PROFILER
        Dictionary<string, Stopwatch> EventStopwatches;
        Dictionary<string, int> EventCount;
        Dictionary<string, float> EventTime;
        List<string> OutputList;

        int stepNumber;
#endif

        public Profiler(bool logToConsole = false)
        {
#if USING_PROFILER
            EventStopwatches = new Dictionary<string, Stopwatch>();
            EventCount = new Dictionary<string, int>();
            EventTime = new Dictionary<string, float>();
            LogToConsole = logToConsole;
            OutputList = new List<string>();

            OutputList.Add("<html><head><title>Profiler Output</title><meta charset=\"utf-8\"/><style>p{padding:0;margin:0;}</style></head><body>");
            OutputList.Add("<h1>Profiler Output</h1>");
            OutputList.Add("<summary>");
            OutputList.Add("<h2>Raw Output</h2>");

            stepNumber = 0;
#endif
        }

        public void LogGameStepStart()
        {
#if USING_PROFILER
            stepNumber++;
            OutputList.Add($"<h3>Game Step Start (step {stepNumber})</h3>");
#endif
        }

        public void LogEventStart(string name)
        {
#if USING_PROFILER
            if (EventStopwatches.ContainsKey(name))
                EventStopwatches[name].Reset();
            else
                EventStopwatches[name] = new Stopwatch();
            EventStopwatches[name].Start();
#endif
        }

        public void LogEventEnd(string name)
        {
#if USING_PROFILER
            if (EventStopwatches.ContainsKey(name))
            {
                EventStopwatches[name].Stop();
                float elapsedTime = EventStopwatches[name].Elapsed.Ticks / 10000f;

                //Add to output
                string output = "Event " + name + ": Total time (ms): " + elapsedTime;
                if (LogToConsole)
                    Console.WriteLine(output);
                OutputList.Add("<p>" + output + "</p>");

                //Add to EventCount and EventTime dictionaries if this is game step 3 or later
                if (stepNumber >= 3)
                {
                    if (!EventCount.ContainsKey(name))
                    {
                        EventCount.Add(name, 0);
                        EventTime.Add(name, 0f);
                    }
                    EventCount[name] += 1;
                    EventTime[name] += elapsedTime;
                }
            }
#endif
        }

        public void ShowOutput()
        {
#if USING_PROFILER
            string output = "";
            foreach (string outputpart in OutputList)
                output += outputpart;

            //Create a summary
            string summary = "<h2>Summary (Game Step 3+)</h2>";
            foreach (KeyValuePair<string, int> eventCountInfo in EventCount)
            {
                summary += $"<p>Event {eventCountInfo.Key}: Count: {eventCountInfo.Value}; Avg Time: {EventTime[eventCountInfo.Key] /eventCountInfo.Value}</p>";
            }
            output = output.Replace("<summary>", summary);

            string path = Path.GetTempPath() + @"\profiler_log.html";
            File.WriteAllText(path, output + "</body></html>", Encoding.UTF8);
            Process.Start(path);
#endif
        }
    }
}
