#!/bin/bash

# Configurações dos seus projetos
INFRA_PROJECT="IvanWeb.Infrastructure"
API_PROJECT="IvanWeb.Api"

# Verifica a ação desejada
if [ "$1" == "add" ]; then
    if [ -z "$2" ]; then
        echo "❌ Erro: Faltou o nome da migration!"
        echo "💡 Uso correto: ./ef-tools.sh add NomeDaMigration"
        exit 1
    fi
    echo "📦 Criando a migration '$2'..."
    dotnet ef migrations add "$2" --project $INFRA_PROJECT --startup-project $API_PROJECT

elif [ "$1" == "update" ]; then
    echo "🚀 Atualizando o banco de dados..."
    dotnet ef database update --project $INFRA_PROJECT --startup-project $API_PROJECT

else
    echo "Comando não reconhecido."
    echo "Como usar o seu automatizador:"
    echo "  ./ef-tools.sh add NomeDaMigration  -> (Para criar uma nova tabela/coluna)"
    echo "  ./ef-tools.sh update               -> (Para aplicar as mudanças no banco)"
fi