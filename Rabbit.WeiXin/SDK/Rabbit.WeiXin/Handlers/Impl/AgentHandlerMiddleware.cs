using Rabbit.WeiXin.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 代理请求模型。
    /// </summary>
    public sealed class AgentRequestModel
    {
        /// <summary>
        /// 初始化一个新的代理请求模型。
        /// </summary>
        /// <param name="uri">代理Uri（绝对的Url地址）。</param>
        /// <param name="retryCount">重试次数。</param>
        /// <param name="timeout">每次请求超时时间（毫秒）。</param>
        public AgentRequestModel(Uri uri, ushort retryCount = 3, ushort timeout = 5000)
        {
            uri.NotNull("uri");

            if (!uri.IsAbsoluteUri || !uri.Scheme.Equals("http"))
                throw new ArgumentException("代理Uri必需是一个绝对的Url地址。", "uri");

            if (timeout == 0)
                throw new ArgumentOutOfRangeException("timeout", timeout, "每次请求超时时间不能小于0毫秒。");

            Timeout = timeout;
            RetryCount = retryCount;
            Uri = uri;
        }

        /// <summary>
        /// 代理Uri（绝对的Url地址）。
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 重试次数。
        /// </summary>
        public ushort RetryCount { get; private set; }

        /// <summary>
        /// 每次请求超时时间（毫秒）。
        /// </summary>
        public ushort Timeout { get; private set; }
    }

    /// <summary>
    /// 代理请求处理中间件。
    /// </summary>
    public sealed class AgentHandlerMiddleware : HandlerMiddleware
    {
        private readonly AgentRequestModel _agentRequestModel;

        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        /// <param name="agentRequestModel">代理请求模型。</param>
        public AgentHandlerMiddleware(HandlerMiddleware next, AgentRequestModel agentRequestModel)
            : base(next)
        {
            _agentRequestModel = agentRequestModel;
        }

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            var request = context.Request;

            //HTTP正文。
            var bytes = context.Request.InputStream.ReadBytes();

            //根据查询字符串变量集合得到一个参数字典表。
            var parameters = request.QueryString.AllKeys.ToDictionary(i => i, i => request.QueryString[i]);
            //得到最终的请求Url。
            var postUrl = AppendParameter(_agentRequestModel.Uri.ToString(), parameters);

            Func<Task<string>> getPostTask =
                () =>
                {
                    var task = Task.Factory.StartNew(() => Encoding.UTF8.GetString(HttpHelper.Post(postUrl, bytes)));
                    //防止出错时程序崩溃。
                    task.ContinueWith(t => { }, TaskContinuationOptions.OnlyOnFaulted);
                    return task;
                };

            string content = null;
            Exception exception = null;
            //开始请求。
            for (var i = 0; i < _agentRequestModel.RetryCount + 1/*重试次数加上一次必需请求的次数*/; i++)
            {
                var task = getPostTask();
                var isTimeout = !task.Wait(_agentRequestModel.Timeout);

                exception = task.Exception;
                //超时或者失败则进行尝试。
                if (isTimeout || task.IsFaulted)
                    continue;

                content = task.Result;
                //结束请求。
                break;
            }

            if (content == null)
                throw new Exception("代理请求次数超过了重试次数并且目标方还是没有正确响应" + (exception == null ? "。" : "错误消息：" + exception.Message));

            context.ResponseXml = content;

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware

        #region Private Method

        private static string AppendParameter(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var builder = new StringBuilder(url);

            builder.Append(!url.Contains("?") ? "?" : "&");

            foreach (var parameter in parameters)
            {
                builder
                    .Append(parameter.Key)
                    .Append("=")
                    .Append(parameter.Value)
                    .Append("&");
            }

            return builder.ToString(0, builder.Length - 1);
        }

        #endregion Private Method
    }
}