# 屏幕使用时间  

## 简介  
一个电脑屏幕使用时间统计工具。

![alt text](assets/screenshot1.png)

![alt text](assets/screenshot2.png)


### 功能

- 统计电脑上不同应用占据最上层窗口时的时间。
- 开启空闲检测后可在长时间未操作时停止记录。
- 支持自定义每天开始和结束的时间。
- 展示数据时可排除指定程序/类别。。
- 可以详细查看某程序/类别的使用情况
- 可以查看某天使用情况的时间线。
- 可方便的设置开机自启动。

## 联系方式
QQ群：745018774

## 开发
+ 准备环境
  + .NET SDK 10.0+
  + Node.js 22.12.0+
+ 克隆代码库：
  ```shell
    git clone https://github.com/majianchuan/ScreenTimeTracker.git
  ```
+ 构建界面库：
  ```shell
  cd src/frontend
  pnpm install
  pnpm build
  ```
+ 启动应用：
  ```shell
  cd ../Hosts/Desktop
  dotnet run
  ```
