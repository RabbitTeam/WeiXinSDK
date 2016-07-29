using Rabbit.WeiXin.Open.Messages.Events;
using Rabbit.WeiXin.Utility.Extensions;

namespace Rabbit.WeiXin.Open.Api
{
    /// <summary>
    /// 一个抽象的核实票据服务。
    /// </summary>
    public interface IVerifyTicketService
    {
        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="model">核实票据模型。</param>
        void Set(ComponentVerifyTicketPush model);

        /// <summary>
        /// 获取核实票据信息。
        /// </summary>
        /// <returns>核实票据模型。</returns>
        ComponentVerifyTicketPush Get();
    }

    /// <summary>
    /// 核实票据服务实现。
    /// </summary>
    public sealed class VerifyTicketService : IVerifyTicketService
    {
        #region Field

        private ComponentVerifyTicketPush _model;

        #endregion Field

        #region Implementation of IVerifyTicketService

        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="model">核实票据模型。</param>
        public void Set(ComponentVerifyTicketPush model)
        {
            _model = model.NotNull("model");
        }

        /// <summary>
        /// 获取核实票据信息。
        /// </summary>
        /// <returns>核实票据模型。</returns>
        public ComponentVerifyTicketPush Get()
        {
            return _model;
        }

        #endregion Implementation of IVerifyTicketService
    }
}