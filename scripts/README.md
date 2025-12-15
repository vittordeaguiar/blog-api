# 🗺️ Sistema de Roadmap Automático

Este diretório contém scripts para gerar e manter o `ROADMAP.md` atualizado automaticamente a partir do **GitHub Projects (Kanban)**.

## 📋 Como Funciona

O sistema se conecta ao seu **GitHub Project** via API GraphQL e busca o status de cada issue diretamente do board Kanban.

### Estados das Issues (Baseado nas Colunas do Kanban)

| Status | Critério | Descrição |
|--------|----------|-----------|
| 🔄 **In Progress** | Issues na coluna "In Progress" | Issues atualmente sendo trabalhadas |
| 🚀 **Ready** | Issues na coluna "Ready" | Issues prontas para serem iniciadas |
| 📋 **Backlog** | Issues na coluna "Backlog" | Issues planejadas para o futuro |
| ✅ **Done** | Issues na coluna "Done" ou fechadas | Issues finalizadas |
| ⚪ **Sem Status** | Issues sem coluna definida | Issues não organizadas no Kanban |

## ⚙️ Configuração

O script está configurado para o projeto:
- **Nome do Projeto**: "Blog API"
- **Owner**: vittordeaguiar
- **Colunas**: Backlog, Ready, In Progress, Done

Para alterar essas configurações, edite as variáveis no início do script `generate-roadmap.sh`:

```bash
PROJECT_NAME="Blog API"
OWNER="vittordeaguiar"
```

## 🚀 Uso

### Execução Manual

Para gerar ou atualizar o ROADMAP manualmente:

```bash
./scripts/generate-roadmap.sh
```

### Automático via GitHub Actions

O workflow `.github/workflows/update-roadmap.yml` executa automaticamente em:

- ✅ Quando uma issue é criada, editada, fechada ou tem labels modificadas
- ✅ Quando há push na branch `main`
- ✅ Manualmente via GitHub Actions UI (workflow_dispatch)
- ✅ Quando o status de uma issue é alterado no Kanban

O bot fará commit automático do ROADMAP.md atualizado se houver mudanças.

## 📦 Dependências

### GitHub CLI (gh)

O script requer o GitHub CLI instalado e autenticado com permissões de Projects:

**Instalação no macOS:**
```bash
brew install gh
```

**Autenticação com scope de Projects:**
```bash
gh auth login
gh auth refresh -s read:project
```

O script irá solicitar automaticamente a permissão `read:project` se não estiver presente.

### jq (JSON processor)

Usado para processar respostas JSON da API GraphQL do GitHub:

**Instalação no macOS:**
```bash
brew install jq
```

## 🎯 GitHub Projects (Kanban)

### Como Criar o Project

1. Acesse: `https://github.com/users/vittordeaguiar/projects`
2. Clique em "New project"
3. Escolha o template "Board" (Kanban)
4. Nomeie como "Blog API"
5. Crie as colunas: **Backlog**, **Ready**, **In Progress**, **Done**

### Como Adicionar Issues ao Project

**Via Interface Web:**
1. Abra o project
2. Clique em "+ Add item"
3. Selecione a issue
4. Arraste para a coluna apropriada

**Via GitHub CLI:**
```bash
# Listar projetos
gh project list

# Adicionar issue ao projeto
gh project item-add <PROJECT_NUMBER> --owner vittordeaguiar --url https://github.com/vittordeaguiar/blog-api/issues/2

# Mover issue para coluna específica
gh project item-edit --project-id <PROJECT_ID> --id <ITEM_ID> --field-id <STATUS_FIELD_ID> --value "In Progress"
```

## 📝 Formato do ROADMAP.md

O arquivo gerado inclui:

- 📊 **Status Geral**: Tabela com contagem de issues por status do Kanban
- 🔄 **In Progress**: Issues na coluna "In Progress"
- 🚀 **Ready**: Issues na coluna "Ready"
- 📋 **Backlog**: Issues na coluna "Backlog"
- ⚪ **Sem Status**: Issues não adicionadas ao Kanban (se houver)
- ✅ **Done**: Issues na coluna "Done" ou fechadas
- 🔗 **Links diretos**: Cada issue tem link para a issue no GitHub
- 🏷️ **Milestones**: Exibidos quando presentes

## 🔧 Customização

### Modificar Nome do Projeto

Edite as variáveis no início do script:

```bash
PROJECT_NAME="Seu Projeto"
OWNER="seu-usuario"
```

### Adicionar Novas Colunas

1. Edite a seção de filtragem no script (linhas 163-167):

```bash
BACKLOG=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Backlog")]')
READY=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Ready")]')
IN_PROGRESS=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "In Progress")]')
DONE=$(echo "$ISSUES_JSON" | jq -s '[.[] | select(.status == "Done" or .state == "CLOSED")]')
```

2. Adicione novas seções no template do ROADMAP

3. Atualize a tabela de Status Geral

### Modificar Formato

O template do ROADMAP está nas seções `cat > "$OUTPUT_FILE"` e `cat >> "$OUTPUT_FILE"` do script.

## 🐛 Troubleshooting

### "GitHub CLI (gh) não encontrado"
- Instale o gh CLI: `brew install gh`

### "Você não está autenticado"
- Execute: `gh auth login`

### "your authentication token is missing required scopes [read:project]"
- Execute: `gh auth refresh -s read:project`
- O script tenta fazer isso automaticamente

### "Projeto 'Blog API' não encontrado"
- Certifique-se que o projeto existe em `https://github.com/users/vittordeaguiar/projects`
- Verifique se o nome está correto (case-sensitive)
- O script lista os projetos disponíveis ao falhar

### "jq não encontrado"
- Instale o jq: `brew install jq`

### "Não foi possível detectar o repositório"
- Certifique-se de estar no diretório correto
- Verifique se o remote do GitHub está configurado: `git remote -v`

### GitHub Action não está executando
- Verifique as permissões do workflow no GitHub
- Certifique-se que Actions está habilitado no repositório
- Adicione permissão `read:project` nas configurações do repositório
- Veja os logs em: `https://github.com/vittordeaguiar/blog-api/actions`

### Issues não aparecem no ROADMAP
- Certifique-se que as issues foram adicionadas ao GitHub Project
- Verifique se as colunas do Kanban têm os nomes exatos: "Backlog", "Ready", "In Progress", "Done"
- Os nomes são case-sensitive!

## 🔐 Permissões

### GitHub CLI (local)
- `repo` - Para ler informações do repositório
- `read:project` - Para ler dados do GitHub Projects

### GitHub Actions
O workflow requer:
- `contents: write` - Para commitar o ROADMAP.md
- `issues: read` - Para ler as issues do repositório
- `projects: read` - Para ler dados do GitHub Projects

Estas permissões são configuradas no arquivo `update-roadmap.yml`.

## 🎯 Diferenças vs Sistema com Labels

| Aspecto | Sistema Anterior (Labels) | Sistema Atual (Projects) |
|---------|---------------------------|--------------------------|
| **Organização** | Labels nas issues | Colunas no Kanban board |
| **Visualização** | Apenas nas issues | Board visual + ROADMAP |
| **Manutenção** | Manual por issue | Drag & drop no board |
| **Integração** | API simples | GraphQL API |
| **Flexibilidade** | Limitada a labels | Múltiplos campos customizados |
| **Permissões** | Incluída no `repo` | Requer `read:project` |

## 📚 Recursos Adicionais

- [GitHub CLI Documentation](https://cli.github.com/manual/)
- [GitHub Projects v2 Documentation](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [GitHub GraphQL API](https://docs.github.com/en/graphql)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [jq Manual](https://stedolan.github.io/jq/manual/)

---

**Desenvolvido para o projeto Blog API** 🚀
