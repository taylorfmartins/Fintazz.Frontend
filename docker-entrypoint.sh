#!/bin/sh
set -e

# Gera appsettings.json com valores de ambiente em runtime
cat > /usr/share/nginx/html/appsettings.json << EOF
{
  "ApiSettings": {
    "BaseUrl": "${API_BASE_URL}"
  }
}
EOF

exec nginx -g "daemon off;"
