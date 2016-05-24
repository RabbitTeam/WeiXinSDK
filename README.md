# 支持的消息类型
## 请求消息
参考文档：http://mp.weixin.qq.com/wiki/10/79502792eef98d6e0c6e1739da387346.html

1. RequestMessageImage（图片消息）
2. RequestMessageLink（链接消息）
3. RequestMessageLocation（位置消息）
4. RequestMessageShortVideo（短视频消息）
5. RequestMessageText（文本消息）
6. RequestMessageVideo（视频消息）
7. RequestMessageVoice（语音消息）

## 事件消息
由于较多直接给出GitHub地址：
https://github.com/RabbitTeam/WeiXinSDK/tree/master/Rabbit.WeiXin/SDK/Rabbit.WeiXin/MP/Messages/Events

## 响应消息
1. ResponseMessageImage（图片消息）
2. ResponseMessageMusic（音乐消息）
3. ResponseMessageNews（图文消息）
4. ResponseMessageText（文本消息）
5. ResponseMessageTransferCustomerService（多客服消息）
6. ResponseMessageVideo（视频消息）
7. ResponseMessageVoice（语音消息）

# 消息处理中间件
1. SignatureCheckHandlerMiddleware（验证签名中间件）
2. CreateRequestMessageHandlerMiddleware（创建消息中间件）
3. SessionSupportHandlerMiddleware（会话支持中间件）
4. IgnoreRepeatMessageHandlerMiddleware（忽略重复的消息中间件）
5. GenerateResponseXmlHandlerMiddleware（生成相应XML处理中间件）
6. AgentHandlerMiddleware（代理请求中间件）

# 支持的API
1. 基础接口
       1. 获取access token 
       2. 获取微信服务器IP地址
2. 发送消息
       1. 客服接口（<http://mp.weixin.qq.com/wiki/1/70a29afed17f56d537c833f89be979c9.html>）
       2. 高级群发接口（<http://mp.weixin.qq.com/wiki/15/5380a4e6f02f2ffdc7981a8ed7a40753.html>）
       3. 模板消息接口（<http://mp.weixin.qq.com/wiki/17/304c1885ea66dbedf7dc170d84999a9d.html>）
3. 素材管理
4. 用户管理
5. 自定义菜单
6. 账号管理
7. 多客服接口
       1. 客服管理
       2. 多客服会话控制
       3. 获取客服聊天记录
8. 微信门店接口
9. 微信卡券接口（部分）
      1. 上传卡券Logo
      2. 创建卡券
      3. 获取卡券可用颜色
10. 微信卡券接口
       1. 投放卡券
       2. 核销卡券
       3. 管理卡券
       4. 卡券事件推送

# 暂不支持的API
1. 数据统计接口
2. 微信小店接口
3. 微信智能接口
4. 摇一摇周边

# 关于性能
在之前的文章就有提及新的SDK是比较追求性能的而在beta1版本中通过一些性能测试还算是达标，后续我会继续在性能上做足优化，争取提升并发量。  
下面是与市面上一个较成熟的微信SDK的性能测试对比：  
**测试环境：**  
> CPU：i7-3610qm  
> 内存：16gb  
> 系统：Windows 8.1 x64  
> 编译配置：Release  
> 迭代次数：10000（一万次）  
> 计数工具：CodeTimer  
> 对比的SDK：暂不透露  

**测试结果：**  
![](http://images0.cnblogs.com/blog/384997/201506/150932036543429.png)

为了防止和用来做对比的SDK粉丝或作者争吵，所以具体的测试代码我不放出了，同样用来被对比的SDK名称我也不公布了，但测试结果绝对公正。

# 关于扩展性
在消息处理过程中采用了管道模式的设计，借鉴了Open Web Interface的思想和一些规范来打造整个消息处理的模型，使消息处理变得更加轻便。  
同时SDK内部内置了一个简单的依赖注入实现 IDependencyResolver，解耦了很多服务与服务实现。  
下面的Demo项目的中的微信请求Action：  
![](http://images0.cnblogs.com/blog/384997/201506/150932047636755.png)

使用者可以自行注册处理的中间件来完成自定的逻辑，后面会专门写具体的使用教程。
# 关于架构
## 消息处理
![](http://images0.cnblogs.com/blog/384997/201506/150932064515441.png)
## 请求消息
![](http://images0.cnblogs.com/blog/384997/201506/150932076237983.png)
## 事件消息
![](http://images0.cnblogs.com/blog/384997/201506/150932091383253.png)
## 响应消息
![](http://images0.cnblogs.com/blog/384997/201506/150932135138749.png)
## 消息格式化器
![](http://images0.cnblogs.com/blog/384997/201506/150932167955148.png)
# Get By Nuget
主要分为两个组件

> **Rabbit.Web**  
> 地址：<https://www.nuget.org/packages/Rabbit.WeiXin/>  
> 命令：Install-Package Rabbit.WeiXin

> **Rabbit.WeiXin.MvcExtension**  
> 地址：<https://www.nuget.org/packages/Rabbit.WeiXin.MvcExtension/>  
> 命令：Install-Package Rabbit.WeiXin.MvcExtension

#测试的微信号
![](http://images0.cnblogs.com/blog/384997/201506/171501158108793.jpg)
# 交流方式
QQ群：384413261  
Email：<majian159@live.com>
