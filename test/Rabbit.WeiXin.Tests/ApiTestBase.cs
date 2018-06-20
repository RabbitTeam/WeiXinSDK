using Rabbit.WeiXin.MP.Api;

namespace Rabbit.WeiXin.Tests
{
    public class ApiTestBase
    {
        #region Field

        //请自行配置。
        private const string AppId = "xxxxxxxx";

        //请自行配置。
        private const string AppSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        //请自行配置。
        protected const string OpenId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        protected readonly ICommonService CommonService;

        protected AccountModel AccountModel;

        #endregion Field

        #region Constructor

        public ApiTestBase()
        {
            AccountModel = new AccountModel
            {
                AppId = AppId,
                AppSecret = AppSecret,
                GetAccessToken = GetAccessToken,
                GetJsApiTicket = GetJsApiTicket,
            };
            CommonService = new CommonService(AccountModel);
        }

        #endregion Constructor

        #region Protected Method

        protected string GetAccessToken()
        {
            return CommonService.GetAccessToken().AccessToken;
        }

        protected string GetJsApiTicket()
        {
            return CommonService.GetJsApiTicket().Ticket;
        }

        #endregion Protected Method
    }
}