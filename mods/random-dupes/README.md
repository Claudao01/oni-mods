# [CLD01] Random Dupes

Versao `1.0.0` do mod que combina nomes e caracteristicas fisicas dos duplicantes disponiveis no jogo.

## Comportamento

- Mantem personalidade, traits, aptidoes, genero, voz e modelo originais.
- Mistura nome exibido, cabelo, olhos, boca e conjunto coerente de pele.
- Separa estritamente os pools Standard e Bionic.
- Usa somente personalidades habilitadas para o conteudo atual.
- Mantem a aparencia original quando um simbolo candidato nao existe.
- Nunca randomiza roupas, outfits, atmo suits ou jet suits.
- Usa a configuracao global como padrao para novas partidas.
- Oferece overrides por colonia em `Pause > Options > Mod Settings` com o host `per-save-mod-settings` instalado.
- Pode adicionar um botao de reroll apenas fisico na Printing Pod.
- Inclui um visualizador animado com botao `Randomize` na tela de opcoes.
- Exporta `available-duplicants.csv` para a pasta de configuracao.
- Registra somente erros do proprio mod em `random-dupes-errors.log`, com timestamp.

O nome ainda pode ser editado normalmente na tela de selecao. Duplicantes ja existentes em um save nao sao randomizados novamente. O logger tenta primeiro a pasta instalada (local ou Steam) e usa a pasta de configuracao como fallback quando ela nao for gravavel.

## Configuracao

- No menu principal, abra `Mods > [CLD01] Random Dupes > Options` para editar os defaults globais.
- Em uma partida, abra `Options > Mod Settings > [CLD01] Random Dupes > Configure` para criar ou editar o override daquela colonia.
- Use `Use global defaults` para remover o override do save.

## Build

```powershell
dotnet build .\CLD01_RandomDupes.csproj -c Release
```

A DLL gerada e `CLD01_RandomDupes.dll`.
