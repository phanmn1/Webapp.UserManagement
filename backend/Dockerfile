# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS base

FROM base AS deps
WORKDIR /source
COPY ./UserManagement/UserManagement.csproj .
RUN dotnet restore
COPY ./UserManagement . 

FROM deps AS development
CMD ["dotnet", "watch", "run", "--no-launch-profile", "--hot-reload-on", "rude-edit"]

FROM deps AS build
# install missing libicu for System.Globalization.Calendars when invariant mode is set to false
RUN dotnet publish -c release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS release
RUN apk add --update icu-libs
WORKDIR /app
COPY --from=build . .
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "AplusCSEMSCore.dll" ]