using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.FileLog.Configuration;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    public class Context_Tests
    {
        private FileLogSettings settings;
        private FileLog.FileLog log = new FileLog.FileLog(TempFileSettings);
        private readonly List<string> createdFiles = new List<string>(2);

        [SetUp]
        public void SetUp()
        {
            settings = new FileLogSettings
            {
                FilePath = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log",
                OutputTemplate = OutputTemplate.Parse("%x %m%n"),
                RollingStrategy = new FileLogSettings.RollingStrategyOptions(),
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
        public void Contextual_log()
        {
            const string prefix = "test";
            var messages = new[] { "Hello, World 1", "Hello, World 2" };
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
        public void Contextual_log_with_subcontext()
        {
            const string prefix1 = "test1";
            const string prefix2 = "test2";
            var messages = new[] {"Hello, World 1", "Hello, World 2"};
            var contextMessages = new[] {$"[{prefix1}] {messages[0]}", $"[{prefix1}] [{prefix2}] {messages[1]}" };

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

        [Test]
        public void Contextual_log_without_prefix_should_work_as_usual_log()
        {
            const string msg = "test message";

            var contextualLog = log.WithContextualPrefix();
            contextualLog.Info(msg);

            WaitForOperationCanceled();
            createdFiles.Add(settings.FilePath);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo($"{string.Empty} {msg}");
        }

        [Test]
        public void Should_write_first_log_with_context_others_without()
        {
            const string msg = "test message";
            const string prefix = "prefix";

            var contextualLog = log.WithContextualPrefix();
            using (new ContextualLogPrefix(prefix))
                contextualLog.Info(msg);
            contextualLog.Info(msg);

            WaitForOperationCanceled();
            createdFiles.Add(settings.FilePath);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo($"[{prefix}] {msg}", $"{string.Empty} {msg}");
        }

        private static void WaitForOperationCanceled() => Thread.Sleep(300);

        private static IEnumerable<string> ReadAllLines(string fileName)
        {
            WaitForOperationCanceled();
            WaitForOperationCanceled();
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(file))
                return reader.ReadToEnd().Split(Environment.NewLine.ToArray()).Where(s => !string.IsNullOrEmpty(s));
        }

        private void UpdateSettings(FileLogSettings settingsPatch)
        {
            settings = settingsPatch;
            log = new FileLog.FileLog(settings);
            WaitForOperationCanceled();
        }

        private void UpdateSettings(Action<FileLogSettings> settingsPatch)
        {
            var copy = new FileLogSettings
            {
                FilePath = settings.FilePath,
                OutputTemplate = settings.OutputTemplate,
                Encoding = settings.Encoding,
                EventsQueueCapacity = settings.EventsQueueCapacity
            };

            settingsPatch(copy);

            UpdateSettings(copy);
        }

        private static FileLogSettings TempFileSettings => new FileLogSettings
        {
            FilePath = "temp",
            RollingStrategy = new FileLogSettings.RollingStrategyOptions(),
            OutputTemplate = OutputTemplate.Parse(string.Empty)
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

        [Test]
        public void FileLog_with_context()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%x %m%n"));

            var conLog = new ContextualPrefixedILogWrapper(log);
            using (new ContextualLogPrefix("prefix1"))
                conLog.Info(messages[0], new { trace = 134 });
            WaitForOperationCanceled();

            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%l %x %p(trace) %m%n"));

            using (new ContextualLogPrefix("prefix2"))
                conLog.Info(messages[1], new { trace = 134 });
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo($"[prefix1] {messages[0]}", $"Info [prefix2] 134 {messages[1]}");
        }

        [Test]
        public void FileLog_with_subcontext()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };
            
            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%x %m%n"));

            var conLog = new ContextualPrefixedILogWrapper(log);
            using (new ContextualLogPrefix("prefix1"))
            using (new ContextualLogPrefix("prefix1.1"))
                conLog.Info(messages[0], new { trace = 134 });
            WaitForOperationCanceled();

            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%l %x %p(trace) %m%n"));

            using (new ContextualLogPrefix("prefix2"))
            using (new ContextualLogPrefix("prefix2.2"))
                conLog.Info(messages[1], new { trace = 134 });
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo($"[prefix1] [prefix1.1] {messages[0]}", $"Info [prefix2] [prefix2.2] 134 {messages[1]}");
        }

        [Test]
        public void FileLog_with_and_without_context()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%x %m%n"));

            var conLog = new ContextualPrefixedILogWrapper(log);
            using (new ContextualLogPrefix("prefix"))
                conLog.Info(messages[0], new { trace = 134 });
            WaitForOperationCanceled();

            UpdateSettings(s => s.OutputTemplate = OutputTemplate.Parse("%l %x %p(trace) %m%n"));

            conLog.Info(messages[1], new { trace = 134 });
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo($"[prefix] {messages[0]}", $"Info 134 {messages[1]}");
        }
    }
}