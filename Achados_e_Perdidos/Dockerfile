# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Dependendo do sistema operacional dos computadores host que compilarão ou executarão os contêineres, a imagem especificada na instrução FROM pode precisar ser alterada.
# Para obter mais informações, consulte https://aka.ms/containercompat.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/runtime:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Achados_e_Perdidos/Achados_e_Perdidos.csproj", "Achados_e_Perdidos/"]
RUN dotnet restore "./Achados_e_Perdidos/Achados_e_Perdidos.csproj"
COPY . .
WORKDIR "/src/Achados_e_Perdidos"
RUN dotnet build "./Achados_e_Perdidos.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Achados_e_Perdidos.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Achados_e_Perdidos.dll"]