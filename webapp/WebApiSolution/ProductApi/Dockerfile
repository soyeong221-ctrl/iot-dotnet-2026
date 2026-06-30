# 이 스테이지는 VS에서 빠른 모드로 실행할 때 사용됩니다(디버그 구성의 기본값).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# USER $APP_UID
WORKDIR /app
EXPOSE 8080

# 이 스테이지는 서비스 프로젝트를 빌드하는 데 사용
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY ["ProductApi.csproj", "."]
#RUN dotnet restore "./ProductApi.csproj"
COPY . .
#WORKDIR "/src/."
#RUN dotnet build "./ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 이 스테이지는 최종 스테이지에 복사할 서비스 프로젝트를 게시하는 데 사용됩니다.
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish 

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ProductApi.dll"]