# Introdução
Projeto de estudos sobre Elasticsearch e BenchmarkDotNet 

## Objetivo
Testes sobre utilização de index de dados no Elasticsearch, integração com .NET, mecanismo de indexação de informações e busca. Também como complemento é feito comparativo entre busca de texto em um banco mysql e index elastic, utilizando a biblioteca BenchmarkDotNet. 

## Sobre o projeto
Nesse repositório encontraremos:
1. docker-compose.yaml que instancia 3 serviços, são eles:
  - elasticsearch - nosso motor de busca e serviço de indexação de dados
  - kibana - para termos visualização e gerenciamento sobre o elasticsearch
  - mysql - banco relacional
2. SampleApi.Elastic, projeto API Rest para testes das métodos de integração entre .NET e Elasticsearch
  - SampleApi.Elastic/Properties - arquivos csv e sql com dados randômicos para inserção no index elastic e tabela mysql
3. SampleBenchmarkElastic, projeto de console .NET que utiliza biblioteca BenchmarkDotNet para realizar comparações entre métodos de busca de texto realizados no elastic e em tabela mysql 

## Guia inicial
No diretório raiz vc deve iniciar serviços docker com comando: 
```
docker-compose up
```
Agora temos 3 serviços sendo executados: elasticsearch, kibana e mysql

No diretório SampleApi.Elastic/ deve executar o comando para executar as migrations no mysql
```
dotnet ef database update
```
Ainda no mesmo diretório já podemos executar a API para testes, comando:
```
dotnet run 
```
Na inicialização já é criado o index default no elastic, chamado `users`
Agora basta acessar o arquivo swagger para testar as rotas. 

#### Importação dos dados
**Elasticsearch:** Realizando uma requisição no endpoint `POST -> /importcsv` o arquivo `MOCK_DATA.csv` é importado para o index padrão no elastic
**Mysql:** Utilizar o arquivo `MOCK_DATA.sql` que possui diversos inserts na tabela `user`

Os 2 arquivos MOCK_DATA possuem os mesmos dados, em formatos diferentes.

#### BenchmarkDotNet
Ir até o diretório SampleBenchmarkElastic e executar o comando: 
```
dotnet run
```
Vai executar, na forma de aplicação console, a instrução do arquivo Program.cs e mostrará um comparativo entre os métodos de busca descritos no arquivo ApiBenchmark.cs

