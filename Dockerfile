# ============================================================
# Stage 1: build - compile Blazor WASM
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY Fintazz.Frontend.slnx .
COPY Fintazz.Web/Fintazz.Web.csproj Fintazz.Web/

RUN dotnet restore Fintazz.Frontend.slnx

COPY . .

RUN dotnet publish Fintazz.Web/Fintazz.Web.csproj \
    -c Release --no-restore -o /app/publish

# ============================================================
# Stage 2: serve with Nginx
# ============================================================
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY docker-entrypoint.sh /docker-entrypoint.sh

RUN chmod +x /docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]
