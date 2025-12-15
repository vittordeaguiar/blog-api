#!/bin/bash

# Script para gerar ROADMAP.md a partir das Issues do GitHub
# Organiza por status do GitHub Projects (Kanban)

set -e

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}📋 Gerando ROADMAP.md a partir do GitHub Projects...${NC}"

# Configurações do Project
PROJECT_NAME="Blog API"
OWNER="vittordeaguiar"

# Verifica se gh CLI está instalado
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI (gh) não encontrado!${NC}"
    echo "Instale com: brew install gh"
    exit 1
fi

# Verifica se jq está instalado
if ! command -v jq &> /dev/null; then
    echo -e "${RED}❌ jq não encontrado!${NC}"
    echo "Instale com: brew install jq"
    exit 1
fi

# Verifica se está autenticado
if ! gh auth status &> /dev/null; then
    echo -e "${YELLOW}⚠️  Você não está autenticado no GitHub CLI${NC}"
    echo "Execute: gh auth login"
    exit 1
fi

# Verifica se tem o scope de project
echo -e "${BLUE}🔑 Verificando permissões...${NC}"
if ! gh auth status 2>&1 | grep -q "read:project"; then
    echo -e "${YELLOW}⚠️  Você precisa adicionar a permissão 'read:project'${NC}"
    echo -e "${YELLOW}Execute: gh auth refresh -h github.com -s read:project${NC}"
    exit 1
fi

# Obtém o nome do repositório
REPO=$(gh repo view --json nameWithOwner -q .nameWithOwner 2>/dev/null || echo "")
if [ -z "$REPO" ]; then
    echo -e "${RED}❌ Não foi possível detectar o repositório GitHub${NC}"
    echo "Certifique-se de estar em um diretório com repositório GitHub configurado"
    exit 1
fi

echo -e "${GREEN}📦 Repositório: $REPO${NC}"
echo -e "${BLUE}🗂️  Projeto: $PROJECT_NAME${NC}"

# Busca o ID do projeto usando GraphQL
echo -e "${YELLOW}🔍 Buscando projeto...${NC}"

PROJECT_QUERY=$(cat <<EOF
query {
  user(login: "$OWNER") {
    projectsV2(first: 20) {
      nodes {
        id
        title
        number
      }
    }
  }
}
EOF
)

PROJECT_DATA=$(gh api graphql -f query="$PROJECT_QUERY" 2>/dev/null || echo "")

if [ -z "$PROJECT_DATA" ] || [ "$PROJECT_DATA" = "{}" ]; then
    echo -e "${RED}❌ Erro ao buscar projetos${NC}"
    exit 1
fi

# Extrai o ID do projeto
PROJECT_ID=$(echo "$PROJECT_DATA" | jq -r --arg name "$PROJECT_NAME" '.data.user.projectsV2.nodes[] | select(.title == $name) | .id')

if [ -z "$PROJECT_ID" ] || [ "$PROJECT_ID" = "null" ]; then
    echo -e "${RED}❌ Projeto '$PROJECT_NAME' não encontrado${NC}"
    echo -e "${YELLOW}Projetos disponíveis:${NC}"
    echo "$PROJECT_DATA" | jq -r '.data.user.projectsV2.nodes[].title'
    exit 1
fi

echo -e "${GREEN}✅ Projeto encontrado: $PROJECT_ID${NC}"

# Busca os items do projeto com seus status
echo -e "${YELLOW}🔍 Buscando issues do projeto...${NC}"

ITEMS_QUERY=$(cat <<EOF
query {
  node(id: "$PROJECT_ID") {
    ... on ProjectV2 {
      items(first: 100) {
        nodes {
          id
          fieldValues(first: 20) {
            nodes {
              ... on ProjectV2ItemFieldSingleSelectValue {
                name
                field {
                  ... on ProjectV2SingleSelectField {
                    name
                  }
                }
              }
            }
          }
          content {
            ... on Issue {
              number
              title
              url
              state
              milestone {
                title
              }
            }
          }
        }
      }
    }
  }
}
EOF
)

ITEMS_DATA=$(gh api graphql -f query="$ITEMS_QUERY" 2>/dev/null || echo "{}")

# Processa os dados e organiza por status
echo -e "${YELLOW}📊 Organizando por status...${NC}"

# Extrai issues e seus status do Kanban
ISSUES_JSON=$(echo "$ITEMS_DATA" | jq -c '
  .data.node.items.nodes[] |
  select(.content.number != null) |
  {
    number: .content.number,
    title: .content.title,
    url: .content.url,
    state: .content.state,
    milestone: .content.milestone,
    status: (
      [.fieldValues.nodes[] |
       select(.field.name == "Status") |
       .name
      ] | first // "No Status"
    )
  }
')

# Separa por status do Kanban
BACKLOG=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Backlog")]')
READY=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Ready")]')
IN_PROGRESS=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "In progress")]')
DONE=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Done" or .state == "CLOSED")]')
NO_STATUS=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "No Status" and .state != "CLOSED")]')

# Função para formatar issues
format_issues() {
    local issues=$1
    local status=$2

    if [ "$issues" = "[]" ] || [ -z "$issues" ]; then
        echo ""
        echo "_Nenhuma issue nesta categoria_"
        echo ""
        return
    fi

    echo "$issues" | jq -r '.[] |
        "- [#\(.number)](\(.url)) - \(.title)" +
        (if .milestone then " `\(.milestone.title)`" else "" end)
    '
    echo ""
}

# Gera o arquivo ROADMAP.md
OUTPUT_FILE="ROADMAP.md"
TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')

cat > "$OUTPUT_FILE" << EOF
# 🗺️ Roadmap - Blog API

> Última atualização: $TIMESTAMP
> Gerado automaticamente a partir do [GitHub Project]($REPO/projects) e [Issues]($REPO/issues)

---

## 📊 Status Geral

EOF

# Conta issues por status
BACKLOG_COUNT=$(echo "$BACKLOG" | jq 'length')
READY_COUNT=$(echo "$READY" | jq 'length')
IN_PROGRESS_COUNT=$(echo "$IN_PROGRESS" | jq 'length')
DONE_COUNT=$(echo "$DONE" | jq 'length')
NO_STATUS_COUNT=$(echo "$NO_STATUS" | jq 'length')
TOTAL=$((BACKLOG_COUNT + READY_COUNT + IN_PROGRESS_COUNT + DONE_COUNT + NO_STATUS_COUNT))

cat >> "$OUTPUT_FILE" << EOF
| Status | Quantidade |
|--------|------------|
| ✅ Done | $DONE_COUNT |
| 🔄 In Progress | $IN_PROGRESS_COUNT |
| 🚀 Ready | $READY_COUNT |
| 📋 Backlog | $BACKLOG_COUNT |
| ⚪ Sem Status | $NO_STATUS_COUNT |
| **Total** | **$TOTAL** |

---

## 🔄 In Progress

Issues atualmente sendo desenvolvidas:

EOF

format_issues "$IN_PROGRESS" "in_progress" >> "$OUTPUT_FILE"

cat >> "$OUTPUT_FILE" << EOF
---

## 🚀 Ready

Issues prontas para serem iniciadas:

EOF

format_issues "$READY" "ready" >> "$OUTPUT_FILE"

cat >> "$OUTPUT_FILE" << EOF
---

## 📋 Backlog

Issues planejadas para o futuro:

EOF

format_issues "$BACKLOG" "backlog" >> "$OUTPUT_FILE"

if [ "$NO_STATUS_COUNT" -gt 0 ]; then
cat >> "$OUTPUT_FILE" << EOF
---

## ⚪ Sem Status

Issues que ainda não foram organizadas no Kanban:

EOF

format_issues "$NO_STATUS" "no_status" >> "$OUTPUT_FILE"
fi

cat >> "$OUTPUT_FILE" << EOF
---

## ✅ Done

Issues concluídas:

EOF

format_issues "$DONE" "done" >> "$OUTPUT_FILE"

cat >> "$OUTPUT_FILE" << EOF
---

## 📝 Legendas

- 🔄 **In Progress**: Issues sendo trabalhadas ativamente
- 🚀 **Ready**: Issues prontas para desenvolvimento
- 📋 **Backlog**: Issues planejadas para o futuro
- ✅ **Done**: Issues finalizadas
- ⚪ **Sem Status**: Issues não categorizadas no Kanban

## 🔧 Como Atualizar

Execute o script:
\`\`\`bash
./scripts/generate-roadmap.sh
\`\`\`

Ou aguarde a atualização automática via GitHub Actions (a cada push ou alteração de issue/project).

---

_Baseado no GitHub Project "${PROJECT_NAME}" (Kanban)_
EOF

echo -e "${GREEN}✅ ROADMAP.md gerado com sucesso!${NC}"
echo -e "${GREEN}📄 Issues processadas: $TOTAL${NC}"
echo -e "   - ✅ Done: $DONE_COUNT"
echo -e "   - 🔄 In Progress: $IN_PROGRESS_COUNT"
echo -e "   - 🚀 Ready: $READY_COUNT"
echo -e "   - 📋 Backlog: $BACKLOG_COUNT"
if [ "$NO_STATUS_COUNT" -gt 0 ]; then
    echo -e "   - ⚪ Sem Status: $NO_STATUS_COUNT"
fi
