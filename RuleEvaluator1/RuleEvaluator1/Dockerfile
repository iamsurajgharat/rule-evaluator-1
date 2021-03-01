FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["RuleEvaluator1/RuleEvaluator1.csproj", "RuleEvaluator1/"]
RUN dotnet restore "RuleEvaluator1/RuleEvaluator1.csproj"
COPY . .
WORKDIR "/src/RuleEvaluator1"
RUN dotnet build "RuleEvaluator1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RuleEvaluator1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuleEvaluator1.dll"]