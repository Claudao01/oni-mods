# Random Dupes

Versao inicial `0.1.0` do mod que combina nomes e partes visuais dos duplicantes disponiveis no jogo.

## Comportamento

- Mantem personalidade, traits, aptidoes, genero, voz e modelo originais.
- Mistura nome exibido, cabelo, rosto, corpo e pele.
- Separa estritamente os pools Standard e Bionic.
- Usa somente personalidades habilitadas para o conteudo atual.
- Mantem a aparencia original quando um simbolo candidato nao existe.

O nome ainda pode ser editado normalmente na tela de selecao. Duplicantes ja existentes em um save nao sao randomizados novamente.

## Build

```powershell
dotnet build .\CLD01_RandomDupes.csproj -c Release
```

A DLL gerada e `CLD01_RandomDupes.dll`.
