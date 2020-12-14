using FsChat.Providers.Chat.Settings;
using FsChat.Providers.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace FsChat.Providers.Chat.Test
{
    [TestClass]
    public class ChatSessionQueueTests
    {
        [TestMethod]
        public void EnqueueAndEndSessionTest()
        {
            var sessionId = Guid.NewGuid();
            var queue = new ChatSessionQueue(new ConsoleLogProvider(), new AgentChatCoordinatorService(new ConsoleLogProvider()));
            Assert.IsTrue(queue.Enqueue(new ChatSession(sessionId)));

            queue.RemoveSession(sessionId);
            Assert.IsNull(queue.GetChatSession(sessionId));
        }

        [TestMethod]
        public void MaxCapacityTest()
        {
            var agentChatCoordinatorService = new AgentChatCoordinatorService(new ConsoleLogProvider());
            var queue = new ChatSessionQueue(new ConsoleLogProvider(), agentChatCoordinatorService);
            var teamCapacity = agentChatCoordinatorService.GetTeamCapacity();

            for (var i = 0; i < teamCapacity + 1; i++)
            {
                var sessionId = Guid.NewGuid();

                if (i < teamCapacity)
                {
                    // Ensure that when within the capacity, the enqueue is true
                    Assert.IsTrue(queue.Enqueue(new ChatSession(sessionId)));
                }
                else
                {
                    // Ensure that when we exceed the capacity, the enqueue is false
                    Assert.IsFalse(queue.Enqueue(new ChatSession(sessionId)));
                }
            }
        }

        [TestMethod]
        public void SessionKeepAliveTest()
        {
            var agentChatCoordinatorService = new AgentChatCoordinatorService(new ConsoleLogProvider());
            var queue = new ChatSessionQueue(new ConsoleLogProvider(), agentChatCoordinatorService);
            var sessionId = Guid.NewGuid();
            Assert.IsTrue(queue.Enqueue(new ChatSession(sessionId)));

            for (var i = 0; i < 10; i++)
            {
                queue.UpdateHeartbeat(sessionId);
                Assert.IsNotNull(queue.GetChatSession(sessionId));
                if (i < 9)
                {
                    Thread.Sleep(1000);
                }
            }
            Assert.IsNotNull(queue.GetChatSession(sessionId));
            queue.RemoveSession(sessionId);
            Assert.IsNull(queue.GetChatSession(sessionId));
        }

        [TestMethod]
        public void SessionExpiryTest()
        {
            var agentChatCoordinatorService = new AgentChatCoordinatorService(new ConsoleLogProvider());
            var queue = new ChatSessionQueue(new ConsoleLogProvider(), agentChatCoordinatorService);
            var sessionId = Guid.NewGuid();
            Assert.IsTrue(queue.Enqueue(new ChatSession(sessionId)));

            Thread.Sleep((ChatSettings.ChatSessionTimeoutSeconds + 2) * 1000);
            Assert.IsNull(queue.GetChatSession(sessionId));
        }
    }
}
