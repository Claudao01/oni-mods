# ONI Mods - Repositório de Desenvolvimento

Monorepo para desenvolvimento de mods para **Oxygen Not Included (ONI)** usando C#, Harmony e a arquitetura do KMod.

## Estrutura do Repositório

```text
oni-mods/
├── Directory.Build.props          # Configuraçãoo central compartilhada
├── ONI_Mods.slnx                  # Solução Visual Studio para edição IDE
├── mods/
│   └──{NomeDoMod}/
│       ├── {NomeDoMod}.csproj     # Arquivo de projeto simplificado
│       ├── *.cs                   # Classes C# com patches Harmony
│       └── bin/Debug/             # DLLs compiladas (gitignored)
├── .game-libs/                    # DLLs do jogo (Assembly-CSharp, Harmony, etc.)
└── README.md                      # Este arquivo
```

## Como Compilar um Mod

### 1. Criar um novo mod
Crie um subdiretório em `mods/{NomeDoMod}/`:

```bash
mkdir oni-mods\mods\MeuMod
```

### 2. Arquivo `.csproj` mínimo

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>MeuMod</AssemblyName>
    <RootNamespace>MeuMod</RootNamespace>
  </PropertyGroup>
</Project>
```

Automaticamente herda:
- TargetFramework (`net48`)
- Referências de DLLs do jogo (Assembly-CSharp, UnityEngine, 0Harmony, etc.)
- Configurações de compilação

### 3. Compilar

```bash
cd oni-mods\mods\MeuMod
dotnet build
```

A DLL compilada estará em: `bin\Debug\MeuMod.dll`

## Como Testar Mods Localmente

### Pré-requisitos
- ONI instalado localmente (Steam ou Epic Games)
- Pasta de mods do jogo identificada

### 1. Localizar a pasta de mods do ONI

**Windows:**
```
C:\Users\{SeuUsuário}\AppData\LocalLow\Klei\Oxygen Not Included\mods
```

Ou procure pelo launcher/settings do ONI.

### 2. Copiar o mod compilado

Após compilar, copie a DLL para a pasta de mods:

```bash
xcopy "oni-mods\mods\MeuMod\bin\Debug\MeuMod.dll" "%APPDATA%\LocalLow\Klei\Oxygen Not Included\mods\" /Y
```

Ou manualmente:
1. Vá em `bin\Debug\MeuMod.dll`
2. Cole na pasta de mods do ONI
3. Reinicie o jogo

### 3. Verificar se o mod carregou

**No console/log do jogo:**
- Abra `output_log.txt` (em `%APPDATA%\LocalLow\Klei\Oxygen Not Included\`)
- Procure por logs do seu mod (ex.: `Meu primeiro mod ONI carregou!`)
- Se houver erros, procure por `Exception` ou `Error`

**In-game:**
- Vá em **Mods** no menu principal
- Seu mod deve aparecer na lista se o arquivo `mod.yaml` estiver configurado (veja seção de metadados)
- Verifique se está ativado

### 4. Recompilar e atualizar

Após fazer alterações:

```bash
dotnet build
xcopy "bin\Debug\MeuMod.dll" "%APPDATA%\LocalLow\Klei\Oxygen Not Included\mods\" /Y
# Reinicie o jogo para carregar a nova versão
```

## Estrutura de um Mod Tópico

### Ponto de entrada (UserMod2)

```csharp
using HarmonyLib;
using KMod;

namespace MeuMod
{
    public class MeuModMain : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            // Harmony aplica automaticamente todos os [HarmonyPatch]
            // deste assembly aqui
        }
    }
}
```

### Patches Harmony

```csharp
using HarmonyLib;
using UnityEngine;

namespace MeuMod
{
    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    public static class GameInitPatch
    {
        public static void Postfix()
        {
            Debug.Log("Mod foi carregado!");
        }
    }
}
```

### Tipos de patches
- **Prefix**: Executado *antes* do método original. Pode cancelar a execução.
- **Postfix**: Executado *depois* do método original.
- **Transpiler**: Modifica o IL (bytecode) do método.

## Dicas de Desenvolvimento

1. **Use `Debug.Log()`** para debugar. Aparece em `output_log.txt`.
2. **Métodos protected/private** em patches: use `[HarmonyPatch(typeof(Class), "MethodName")]` com string, não `nameof()`.
3. **Referências circulares**: Evite dependências entre mods; use Harmony para desacoplamento.
4. **Testes**: Compile frequentemente. A compilação é rápida (~1s).
5. **Limpar antes de recompilar**: `dotnet clean` se houver problemas de cache.

## Troubleshooting

| Problema | Solução |
|----------|---------|
| Mod não aparece no jogo | Verifique `output_log.txt` por exceções; ensure `mod.yaml` existe |
| Erro "CS0246: namespace not found" | Framework mismatch; check `Directory.Build.props` target framework |
| "The type ... is not accessible" | Método é private/protected; use string em `[HarmonyPatch(..., "MethodName")]` |
| DLL não foi copiada | Verifique a pasta de mods do ONI; use comando `xcopy` acima |
| Jogo não inicia com o mod | Desative na UI de mods; verifique `output_log.txt` por exception stack trace |

## Recursos Adicionais

- **Harmony Docs**: https://harmony.pardeike.net/
- **ONI Modding**: Check the ONI modding community wiki/forums
- **Assembly-CSharp Exploration**: Use dnSpy/ILSpy para inspecionar classes do jogo

---

*Projeto: ONI Mods Generation | Desenvolvido em C# .NET com Harmony*
