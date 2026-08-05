FROM ://microsoft.com AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TTCRestApi.dll"]
