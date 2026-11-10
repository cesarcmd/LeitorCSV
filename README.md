# LeitorCSV

> Projeto de estudo em ASP.NET Core MVC para aprender upload, leitura e visualização de arquivos CSV.

O **LeitorCSV** recebe um CSV e mostra seu conteúdo em uma tabela HTML. Ele não salva o arquivo, não usa banco de dados e não possui contas de usuário.

## Objetivo educacional

Este repositório foi criado para praticar:

- rotas, controllers e Razor Views no ASP.NET Core MVC;
- envio de arquivos com `IFormFile`;
- leitura de CSV com `LumenWorksCsvReader`;
- validação de entrada e controles básicos para um endpoint de upload.

Não envie informações pessoais, sigilosas ou dados reais de uma instituição. Mesmo sem persistência intencional, infraestruturas de hospedagem podem registrar metadados, erros ou tráfego.

## Tecnologias

- .NET 10 e ASP.NET Core MVC
- Razor Views
- [LumenWorksCsvReader](https://www.nuget.org/packages/LumenWorksCsvReader/) 4.0.0
- Bootstrap 5.1.0 e jQuery 3.6.0, incluídos como arquivos estáticos

## Como executar

Pré-requisito: SDK .NET 10.

```bash
dotnet restore
dotnet run
```

Abra o endereço exibido no terminal. No perfil de desenvolvimento, normalmente será `https://localhost:7186` ou `http://localhost:5218`.

Em seguida, escolha **Upload** no menu e envie um CSV compatível.

## Formato esperado

O arquivo precisa estar em UTF-8 (com ou sem BOM), usar vírgulas como separador e ter a primeira linha como cabeçalho.

```csv
Nome,Idade,Cidade
Ana,29,Manaus
Bruno,34,Belém
```

## Limites e controles

Para reduzir riscos no upload, a aplicação aceita:

| Regra | Limite |
| --- | ---: |
| Extensão | `.csv` (maiúsculas/minúsculas aceitas) |
| Tamanho do arquivo | 2 MB |
| Colunas | 1 a 50 |
| Linhas de dados | até 5.000 |
| Tamanho de cada campo | até 10.000 caracteres |
| Frequência | 10 envios por minuto por cliente |

Além disso, a aplicação usa token antifalsificação no envio, HTTPS/HSTS quando não está em desenvolvimento, validação de cabeçalhos, tratamento de CSV inválido e cabeçalhos HTTP de proteção.

O upload permanece sem autenticação por ser um exemplo didático. Para uma implantação corporativa, adicione autenticação, autorização, monitoramento, proteção de borda e uma política formal para dados enviados.

## Publicação em produção

Antes de publicar:

1. Copie `appsettings.Production.json.example` para `appsettings.Production.json`.
2. Substitua `leitorcsv.exemplo.gov.br` pelo domínio real. O arquivo de produção é ignorado pelo Git para evitar publicar configurações locais.
3. Garanta certificado TLS válido e execute com `ASPNETCORE_ENVIRONMENT=Production`.
4. Revise os limites de upload e frequência conforme a capacidade do servidor.
5. Atualize periodicamente o .NET, os pacotes NuGet e as bibliotecas de cliente.

## Estrutura

```text
Controllers/HomeController.cs       # Upload, validações e leitura do CSV
Views/Home/Upload.cshtml            # Formulário e tabela de resultados
Program.cs                          # Serviços, limites, rate limiting e cabeçalhos
appsettings.Production.json.example # Modelo seguro de configuração de produção
SECURITY.md                         # Como relatar vulnerabilidades
```

## Segurança

Consulte [SECURITY.md](SECURITY.md) para o processo de relato responsável de vulnerabilidades.

## Licença

Distribuído sob a [licença MIT](LICENSE).
