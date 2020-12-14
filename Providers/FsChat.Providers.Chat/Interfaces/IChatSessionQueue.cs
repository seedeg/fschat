using FsChat.Providers.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Interfaces
{
    public interface IChatSessionQueue
    {
        /// <summary>
        /// Enqueue the chat session if possible
        /// </summary>
        /// <param name="session">The chat session</param>
        /// <returns>True if the cat has been enqueue and false otherwise</returns>
        bool Enqueue(ChatSession session);

        /// <summary>
        /// Dequeues the next available chat
        /// </summary>
        /// <returns>The dequeued chat session</returns>
        ChatSession Dequeue();

        /// <summary>
        /// Returns a particular chat session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        ChatSession GetChatSession(Guid sessionId);

        /// <summary>
        /// Updates the heartbeat for a particular session
        /// </summary>
        /// <param name="sessionId"></param>
        void UpdateHeartbeat(Guid sessionId);

        /// <summary>
        /// Removes a session from the queue
        /// </summary>
        /// <param name="sessionId">The session ID to remove</param>
        void RemoveSession(Guid sessionId);

        /// <summary>
        /// Checks and returns whether the session queue is full or not
        /// </summary>
        /// <returns>True if full, false otherwise</returns>
        bool IsFull();
    }
}
