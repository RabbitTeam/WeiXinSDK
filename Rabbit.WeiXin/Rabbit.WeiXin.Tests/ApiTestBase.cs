using Rabbit.WeiXin.Api;

namespace Rabbit.WeiXin.Tests
{
    public class ApiTestBase
    {
        #region Field

        private const string AppId = "wxa4ab3e636e2eb702";
        private const string AppSecret = "554a35de9372234868b24b3f084a41bd";
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