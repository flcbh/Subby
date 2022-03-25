FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# sets our working directory in the app folder
WORKDIR /app
# copy the contents of the publish folder into the app folder
COPY ./publish .
# this line tells to docker it should run the dotnet command
ENTRYPOINT ["dotnet", "Subby.Web.dll"]