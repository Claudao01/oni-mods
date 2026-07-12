# ONI Mods

Repositório público de mods para **Oxygen Not Included**, desenvolvidos em C# com Harmony e PLib.

Cada mod vive em `mods/<nome-do-mod>/` e possui código-fonte, metadados, documentação, traduções e instruções próprias. O repositório também concentra releases, acompanhamento de problemas e materiais de referência para a comunidade.

## Mods disponíveis

| Mod | Descrição | Documentação |
| --- | --- | --- |
| Fill with Backwalls | Preenche cavidades fechadas com paredes de fundo. | [README do mod](mods/fill-with-backwalls/README.md) |

## Instalação

Baixe o ZIP da versão desejada em [Releases](https://github.com/Claudao01/oni-mods/releases), extraia a pasta do mod em `Documents\Klei\OxygenNotIncluded\mods\local\` e ative-o pelo menu **Mods** do jogo.

Consulte o README de cada mod para requisitos, compatibilidade, configuração e instruções de uso.

## Problemas e contribuições

Use [Issues](https://github.com/Claudao01/oni-mods/issues) para relatar erros, sugerir melhorias ou solicitar suporte. Ao abrir um problema, informe:

- o mod e a versão usados;
- versão do ONI e DLCs ativos;
- passos para reproduzir;
- trecho relevante de `Player.log`, quando houver;
- capturas de tela ou save de exemplo, se possível.

As issues são organizadas por labels de mod, tipo e prioridade. Consulte as regras e o roteiro de desenvolvimento do repositório antes de enviar uma contribuição.

## Desenvolvimento local

As DLLs proprietárias do ONI não são distribuídas aqui. Para compilar localmente, configure referências válidas para as assemblies do jogo e siga as instruções do README do mod que deseja alterar.

O material introdutório de modding será publicado em [tutorial/](tutorial/README.md).

## Releases

As releases são publicadas por mod. Cada pacote contém somente os arquivos necessários para instalação e possui uma tag SemVer associada. Os detalhes de versão e alterações ficam no `CHANGELOG.md` de cada mod.
