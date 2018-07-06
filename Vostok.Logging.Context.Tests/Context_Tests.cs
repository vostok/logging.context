using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core.Configuration;
using Vostok.Logging.FileLog;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    public class Context_Tests
    {
        private FileLogSettings settings;
        private readonly FileLog.FileLog log = new FileLog.FileLog();
        private readonly List<string> createdFiles = new List<string>(2);

        [SetUp]
        public void SetUp()
        {
            settings = new FileLogSettings
            {
                FilePath = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log",
                ConversionPattern = ConversionPattern.FromString("%m%n"),
                EnableRolling = false,
                AppendToFile = true,
                Encoding = Encoding.UTF8
            };

            UpdateSettings(settings);
        }

        [TearDown]
        public void TearDown()
        {
            UpdateSettings(TempFileSettings);
            log.Info(string.Empty);
            createdFiles.ForEach(DeleteFile);
            createdFiles.Clear();
        }

        [Test]
        public void Contextual_log_test()
        {
            const string prefix = "test";
            var messages = new[] {"Hello, World 1", "Hello, World 2"};
            var contextMessages = messages.Select(m => $"[{prefix}] {m}").ToArray();

            var contextualLog = log.WithContextualPrefix();
            using (new ContextualLogPrefix(prefix))
            {
                contextualLog.Info(messages[0]);
                contextualLog.Info(messages[1]);
            }
            WaitForOperationCanceled();
            createdFiles.Add(settings.FilePath);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(contextMessages);
        }

        [Test]
        public void Contextual_log_test_2()
        {
            const string prefix1 = "test1";
            const string prefix2 = "test2";
            var messages = new[] {"Hello, World 1", "Hello, World 2"};
            var contextMessages = new[] {$"[{prefix1}] {messages[0]}", $"[{prefix2}] {messages[1]}" };

            var contextualLog = log.WithContextualPrefix();
            using (new ContextualLogPrefix(prefix1))
            {
                contextualLog.Info(messages[0]);
                using (new ContextualLogPrefix(prefix2))
                    contextualLog.Info(messages[1]);
            }
            WaitForOperationCanceled();
            createdFiles.Add(settings.FilePath);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(contextMessages);
        }

        private static void WaitForOperationCanceled() => Thread.Sleep(300);

        private static IEnumerable<string> ReadAllLines(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(file))
                return reader.ReadToEnd().Split(Environment.NewLine.ToArray()).Where(s => !string.IsNullOrEmpty(s));
        }

        private void UpdateSettings(FileLogSettings settingsPatch)
        {
            settings = settingsPatch;
            FileLog.FileLog.Configure(settings);
            WaitForOperationCanceled();
        }

        private void UpdateSettings(Action<FileLogSettings> settingsPatch)
        {
            var copy = new FileLogSettings
            {
                FilePath = settings.FilePath,
                ConversionPattern = settings.ConversionPattern,
                EnableRolling = settings.EnableRolling,
                AppendToFile = settings.AppendToFile,
                Encoding = settings.Encoding,
                EventsQueueCapacity = settings.EventsQueueCapacity
            };

            settingsPatch(copy);

            UpdateSettings(copy);
        }

        private static FileLogSettings TempFileSettings => new FileLogSettings
        {
            FilePath = "temp",
            EnableRolling = false,
            ConversionPattern = ConversionPattern.FromString(string.Empty)
        };

        private static void DeleteFile(string fileName)
        {
            while (true)
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    break;
                }
                catch (Exception)
                {
                    WaitForOperationCanceled();
                }
        }
    }
}