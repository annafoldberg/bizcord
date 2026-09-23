# Base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:a04d1c1d2d26119049494057d80ea6cda25bbd8aef7c444a1fc1ef874fd3955b AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:478b9038d187e5b5c29bfa8173ded5d29e864b5ad06102a12106380ee01e2e49 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore project files for caching
COPY ["apps/channel-microservice/src/ChannelService.Presentation/ChannelService.Presentation.csproj",
      "apps/channel-microservice/src/ChannelService.Presentation/"]

COPY ["apps/channel-microservice/src/ChannelService.Infrastructure/ChannelService.Infrastructure.csproj",
      "apps/channel-microservice/src/ChannelService.Infrastructure/"]

COPY ["apps/channel-microservice/src/ChannelService.Application/ChannelService.Application.csproj",
      "apps/channel-microservice/src/ChannelService.Application/"]

COPY ["apps/channel-microservice/src/ChannelService.Domain/ChannelService.Domain.csproj",
      "apps/channel-microservice/src/ChannelService.Domain/"]

COPY ["packages/MessageClient/src/MessageClient.csproj", "../../packages/MessageClient/src/"]
COPY ["packages/Messages/Messages.csproj", "../../packages/Messages"]

RUN dotnet restore "apps/channel-microservice/src/ChannelService.Presentation/ChannelService.Presentation.csproj"

# Copy, build, and output
WORKDIR /src

COPY ["apps/channel-microservice/src/ChannelService.Presentation/ChannelService.Presentation.csproj",
      "apps/channel-microservice/src/ChannelService.Presentation/"]

COPY ["apps/channel-microservice/src/ChannelService.Infrastructure/ChannelService.Infrastructure.csproj",
      "apps/channel-microservice/src/ChannelService.Infrastructure/"]

COPY ["apps/channel-microservice/src/ChannelService.Application/ChannelService.Application.csproj",
      "apps/channel-microservice/src/ChannelService.Application/"]

COPY ["apps/channel-microservice/src/ChannelService.Domain/ChannelService.Domain.csproj",
      "apps/channel-microservice/src/ChannelService.Domain/"]

COPY ["packages/MessageClient/src/MessageClient.csproj", "../../packages/MessageClient/src/"]
COPY ["packages/Messages/Messages.csproj", "../../packages/Messages"]

WORKDIR "apps/channel-microservice/src/ChannelService.Presentation/"

RUN dotnet build "ChannelService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChannelService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bizcord.ChannelService.Presentation.dll"]