
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ChallengeApiAtm.Api/ChallengeApiAtm.Api.csproj", "ChallengeApiAtm.Api/"]
COPY ["ChallengeApiAtm.Application/ChallengeApiAtm.Application.csproj", "ChallengeApiAtm.Application/"]
COPY ["ChallengeApiAtm.Domain/ChallengeApiAtm.Domain.csproj", "ChallengeApiAtm.Domain/"]
COPY ["ChallengeApiAtm.Infrastructure/ChallengeApiAtm.Infrastructure.csproj", "ChallengeApiAtm.Infrastructure/"]

RUN dotnet restore "ChallengeApiAtm.Api/ChallengeApiAtm.Api.csproj"

COPY . .

WORKDIR "/src/ChallengeApiAtm.Api"
RUN dotnet publish "ChallengeApiAtm.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app


RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 3000

ENV ASPNETCORE_URLS=http://+:3000

ENTRYPOINT ["dotnet", "ChallengeApiAtm.Api.dll"] 