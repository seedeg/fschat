using FsChat.Contracts.Data.Response;
using FsChat.Interfaces;
using FsChat.Interfaces.Logging;
using FsChat.Providers.Chat;
using FsChat.Providers.Chat.Interfaces;
using System;
using System.Reflection;
using System.Web.Http;

namespace FsChat.Services.APIs.Public.Controllers
{
    // This API is a basic implemention and does not take advantage of truly asynchronous code
    // For this reason the API methods are not using async await operators
    // We should change to async await when the implementation is done in a way to take advantage of async code
    [RoutePrefix("api/chat")]
    public class ChatController : BaseController
    {
        private readonly IChatSessionQueue chatSessionQueue;

        public ChatController(ILogProvider logProvider,
            IChatSessionQueue chatSessionQueue)
            : base(logProvider)
        {
            this.chatSessionQueue = chatSessionQueue ?? throw new ArgumentNullException(nameof(chatSessionQueue));
        }

        [HttpPost]
        [Route("start-session")]
        public IHttpActionResult StartChatSession()
        {
            try
            {
                var chatSessionId = Guid.NewGuid();
                if (!chatSessionQueue.Enqueue(new ChatSession(chatSessionId)))
                {
                    // No agents are available. Return a 200 and set a generic response with a custom error message 
                    // so we can show a message which is friendly to the user
                    return Json(new GenericResponse
                    {
                        Success = false,
                        ErrorMessage = "No agents are available at this time"
                    });
                }

                return Json(new GenericResponse
                {
                    Success = true,
                    SuccessMessage = chatSessionId.ToString()
                });
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("keep-alive-session/{sessionId}")]
        public IHttpActionResult KeepAliveChatSession([FromUri] Guid sessionId)
        {
            try
            {
                chatSessionQueue.UpdateHeartbeat(sessionId);
                return Ok();
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("end-session/{sessionId}")]
        public IHttpActionResult EndChatSession([FromUri] Guid sessionId)
        {
            try
            {
                chatSessionQueue.RemoveSession(sessionId);
                return Ok();
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
                return InternalServerError();
            }
        }
    }
}
