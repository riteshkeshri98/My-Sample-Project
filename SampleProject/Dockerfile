# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /out -r linux-x64 --self-contained false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# Create appuser with UID 1000
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser

WORKDIR /app
COPY --from=build /out .

# Switch to non-root user
USER appuser

EXPOSE 80
ENTRYPOINT ["dotnet", "SampleProject.dll"]
