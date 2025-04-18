> ### ⚠️ Usage Notice:
> This repository is a course-design project required by ***UESTC***, for reference only.  
> ~~Due to its hasty construction, the author makes no guarantees regarding code quality or actual security, functionalities are purely demonstrative.~~
# Gatito Chat
桌面端跨平台聊天应用，支持远程房间和局域网群组聊天。使用mkPBC作为授权协议，支持加密聊天内容。  
关于mkPBC的详细信息请参考：
[论文笔记 《Password-Based Credentials with Securiy Against Server Compromise》- Twlm's Blog](https://blog.twlmgatito.cn/posts/note-for-mkpbc/)
![main](https://raw.githubusercontent.com/TwilightLemon/Data/refs/heads/master/gatitochat_main.jpg)

## 运行环境
基于.NET 9和AvaloniaUI构建，支持Windows、Linux和macOS

## 安全性
Gatito Auth Server和Chat Server分别使用Https和wss协议与客户端进行通讯，两者分别由Vercel和Microsoft Azure部署在海外，数据持久化由MongoDB Cloud提供。这部分暂无开源计划。  

mkPBC协议保证你提供的用户名(经过hash盲化)只会泄露相同性，而密码确保了强不可伪造性，即使服务器被攻破，也不会泄露关于密码的任何信息。

Chat Server只认可来自Gatito Auth Server的签名Token，有效期为UTC世界时当日，确保用户身份诚实。
用户提供有效邮箱验证后，邮箱信息即从Gatito Auth Server中删除，不会与账号绑定。我们会定期清理不活跃的账号。  
以下是一个服务器端储存的用户信息示例，只有`name`是明文，`uid`由客户端签名并盲化，`pkSign`是服务端公钥。
```json
{
  "name": "twlm",
  "uid": "wjcoNLw8w/WFAUy+MlqALVJVplq/oQxzrpEEtMl1Czk=",
  "pkSign": "-----BEGIN PUBLIC KEY-----\nxxxxxxxxxxxxxx\n-----END PUBLIC KEY-----",
}
```

本来想做群组加密的，后来发现我的能力只能调库，那不如直接用AES对称加密...

## 使用方法
### 运行项目
1. 使用dotnet CLI运行项目
```shell
git clone https://github.com/TwilightLemon/GatitoChat
cd GatitoChat
dotnet run --project GatitoChat
```
2. 使用Visual Studio或Rider打开项目，选择GatitoChat项目运行
3. 使用发布版本(可能有，如果作者不懒的话)

### 通过GatitoAuth登录服务器
在主页面点击用户头像，输入用户名(注意，从2025.4.18开始，所有新注册用户需要验证有效邮箱)，点击'Confirm'，如果没有注册过会自动前往注册，注册后会自动登录。  
![register](https://raw.githubusercontent.com/TwilightLemon/Data/refs/heads/master/gatitochat_register.jpg)

### 创建房间
在主页面点击'Add Room'按钮，选择'Remote'后输入房间名称即可创建远程房间。  
或者可以在局域网内创建服务器，唯一的个性化功能是可以自定义昵称。(因为这不是工作重点)
![local](https://raw.githubusercontent.com/TwilightLemon/Data/refs/heads/master/gatitochat_local.jpg)