using Rabbit.WeiXin.MP.Api;

namespace Rabbit.WeiXin.Tests
{
    public class ApiTestBase
    {
        #region Field

        //请自行配置。
        private const string AppId = "wxa4ab3e636e2eb702";

        //请自行配置。
        private const string AppSecret = "44058117ad949bb778f6d3ac9c89e7ed";

        //请自行配置。
        protected const string OpenId = "oaCoeuN63wMFydbjuZdoV6aPgqm4";

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
                GetAccessToken = GetAccessToken
            };
            CommonService = new CommonService(AccountModel);
        }

        #endregion Constructor

        #region Protected Method

        protected string GetAccessToken()
        {
            return CommonService.GetAccessToken().AccessToken;
        }

        #endregion Protected Method
    }
}