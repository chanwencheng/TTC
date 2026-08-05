# 階段 1: 執行階段基礎映像檔 (使用正確的 mcr 網址)
FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

# 階段 2: SDK 編譯階段
FROM ://microsoft.com AS build
WORKDIR /src

# 複製項目檔並進行還原 (Nuget Restore)
# 註：替換成實際的 .csproj 檔名
COPY ["TTCRestAPI.csproj", "."]
RUN dotnet restore "./TTCRestAPI.csproj"

# 複製所有原始碼並進行編譯
COPY . .
RUN dotnet build "TTCRestAPI.csproj" -c Release -o /app/build

# 階段 3: 發布階段
FROM build AS publish
RUN dotnet publish "TTCRestAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 階段 4: 最終執行映像檔
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TTCRestAPI.dll"]