FROM mcr.microsoft.com/mssql/server:2022-latest
EXPOSE 1433

ENV SA_PASSWORD="@Admin123"
ENV ACCEPT_EULA="Y"
ENV MSSQL_PID="Developer"
ENV DATABASE_NAME="asp_net_10_erudio"

COPY RestWithASPNET10Erudio/db/migrations/*.sql /home/database/
COPY RestWithASPNET10Erudio/db/dataset/*.sql /home/database/
COPY RestWithASPNET10Erudio/ci/init_database.sh /docker-entrypoint-initdb.d/init_database.sh
