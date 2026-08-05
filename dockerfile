# 階段 1: 執行階段基礎映像檔 (.NET 8)
FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

# 階段 2: SDK 編譯階段 (.NET 8)
FROM ://microsoft.com AS build
WORKDIR /src

# 複製子資料夾內的項目檔並進行還原
COPY ["TTCRestAPI/TTCRestAPI.csproj", "TTCRestAPI/"]
RUN dotnet restore "TTCRestAPI/TTCRestAPI.csproj"

# 複製所有原始碼並進行編譯
COPY . .
WORKDIR "/src/TTCRestAPI"
RUN dotnet build "TTCRestAPI.csproj" -c Release -o /app/build

# 階段 3: 發布階段
FROM build AS publish
RUN dotnet publish "TTCRestAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 階段 4: 最終執行映像檔
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TTCRestAPI.dll"]
