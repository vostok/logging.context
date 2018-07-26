using NUnit.Framework;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    public class Context_Tests
    {
        /*private FileLogSettings settings;
        private readonly FileLog log = new FileLog(TempFileSettings);
        private readonly List<string> createdFiles = new List<string>(2);

        [SetUp]
        public void SetUp()
        {
            settings = new FileLogSettings
            {
                FilePath = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log",
                ConversionPattern = ConversionPatternParser.Parse("%x %m%n"),
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
            using (var file = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(file))
                return reader.ReadToEnd().Split(Environment.NewLine.ToArray()).Where(s => !string.IsNullOrEmpty(s));
        }

        private void UpdateSettings(FileLogSettings settingsPatch)
        {
            settings = settingsPatch;
            FileLog.Configure(settings);
            WaitForOperationCanceled();
        }

        private static FileLogSettings TempFileSettings => new FileLogSettings
        {
            FilePath = "temp",
            RollingStrategy = new FileLogSettings.RollingStrategyOptions(),
            ConversionPattern = ConversionPatternParser.Parse(string.Empty)
        };

        private static void DeleteFile(string fileName)
        {
            while (true)
                try
                {
                    if (System.IO.File.Exists(fileName))
                        System.IO.File.Delete(fileName);

                    break;
                }
                catch (Exception)
                {
                    WaitForOperationCanceled();
                }
        }*/
    }
}