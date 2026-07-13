# [CLD01] Per-Save Mod Settings

Host aberto para configuracoes individualizadas por save no Oxygen Not Included.

Durante uma partida, adiciona `Mod Settings` a tela `Options`. A nova tela lista os mods carregados que oferecem configuracao por save, de forma semelhante a lista `Mods` do menu principal.

## Contrato para outros mods

Exponha em qualquer tipo carregado os seguintes metodos estaticos sem argumentos:

```csharp
public static string GetPerSaveOptionsTitle();
public static void ShowPerSaveOptions();
public static void ResetPerSaveOptions(); // opcional
```

O contrato e descoberto por reflexao. O mod consumidor nao precisa referenciar esta DLL e continua funcional quando o host nao estiver instalado.

## Build

```powershell
dotnet build .\CLD01_PerSaveModSettings.csproj -c Release
```
