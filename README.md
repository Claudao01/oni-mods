# ONI Mods

Repositório público de mods para **Oxygen Not Included**, desenvolvido em C# com Harmony e PLib.

## Mods

### Fill with Backwalls

Preenche uma cavidade fechada com a parede de fundo, o material e o visual selecionados.

- código: `mods/fill-with-backwalls/`;
- versão atual: `1.0.0`;
- releases: [GitHub Releases](https://github.com/Claudao01/oni-mods/releases).

## Compilação local

As DLLs do ONI não são distribuídas neste repositório. No workspace de desenvolvimento, elas devem existir em `game/libs/`, conforme os caminhos definidos em `Directory.Build.props`.

```powershell
dotnet build ".\mods\fill-with-backwalls\FillWithBackwalls.csproj" --configuration Release
```

No monorepo privado, o pacote completo é preparado por:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ".\oni-mods\scripts\package-fill-with-backwalls.ps1"
```

O script compila a DLL, valida a versão `1.0.0`, atualiza `release/FillWithBackwalls.dll` e cria o ZIP em `oni-mods/artifacts/`.

## Publicação

O GitHub Actions não compila contra assemblies proprietários do jogo. O workflow valida a DLL preparada localmente, confere as versões do `.csproj`, `mod_info.yaml`, DLL e tag, monta o ZIP e publica a GitHub Release.

Tags de release seguem o formato:

```text
fill-with-backwalls-v1.0.0
```
