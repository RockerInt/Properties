FROM mcr.microsoft.com/mssql/server:2019-latest
# Install Unzip
#USER root
#RUN apt-get update \
    #&& apt-get install unzip -y
# Install SQLPackage for Linux and make it executable
#RUN wget -progress=bar:force -q -O sqlpackage.zip https://go.microsoft.com/fwlink/?linkid=873926 \
    #&& unzip -qq sqlpackage.zip -d /opt/sqlpackage \
    #&& chmod +x /opt/sqlpackage/sqlpackage
# Configure external build arguments to allow configurability.
RUN mkdir -p /var/opt/mssql/database

WORKDIR /var/opt/mssql/database
# Add the DACPAC to the image
#COPY ["DB/Books/Restore/create.sql", "."]
#RUN ls -la .
ARG DBNAME=Properties
ARG PASSWORD
# Configure the required environmental variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=12345678a
# Launch SQL Server, confirm startup is complete, deploy the DACPAC, then terminate SQL Server.
#CMD /opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U SA -P $SA_PASSWORD -i Create.sql