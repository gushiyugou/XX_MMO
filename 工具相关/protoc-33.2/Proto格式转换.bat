@echo off
chcp 65001 > nul

:: 创建输出目录
if not exist "out\csharp" mkdir "out\csharp"

:: 遍历proto目录下的所有.proto文件
for %%f in (proto\*.proto) do (
    echo 正在转换: %%f
    bin\protoc --proto_path=proto --csharp_out=out\csharp %%f
)

echo 转换完成!
pause