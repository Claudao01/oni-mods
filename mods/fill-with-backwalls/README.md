# Fill with Backwalls

Preenche uma cavidade fechada com a parede de fundo, o material e o visual selecionados usando um único clique.

## Recursos

- adiciona a opção **Preencher cavidade** ao seletor de paredes de fundo;
- cria ordens de construção em toda a cavidade fechada;
- substitui o modelo, o material e o visual de paredes de fundo existentes;
- permite limitar o tamanho máximo da cavidade pelo menu **Mods > Fill with Backwalls > Configurar**;
- inclui dicionário em pt-BR, alemão, espanhol, francês, russo, chinês simplificado, japonês e coreano, com inglês como fallback;
- disponibiliza o link do repositório na tela de configuração.

## Instalação manual

1. Baixe o ZIP mais recente na página de Releases.
2. Extraia a pasta `fill-with-backwalls` em `Documents\Klei\OxygenNotIncluded\mods\local\`.
3. Ative **Fill with Backwalls** no menu Mods e reinicie o jogo quando solicitado.

## Uso

1. Selecione uma parede de fundo no menu de construção.
2. Escolha o modelo, o material e o visual desejados.
3. No painel de seleção de modelos e materiais da parede de fundo, marque o checkbox exibido ao lado ou abaixo da lista de materiais.
4. Clique dentro de uma cavidade fechada.

Em pt-BR, o checkbox aparece como **Preencher cavidade**. Em outros idiomas, o nome acompanha a tradução ativa do jogo ou do mod de idioma instalado. O modo permanece ativo até ser desmarcado.

## Configuração

No menu principal, abra **Mods**, localize **Fill with Backwalls** e selecione **Configurar**. O campo **Tamanho máximo da cavidade** define quantas células podem ser preenchidas por clique; o padrão é 120.

A tela de configuração também mostra um acesso ao repositório do mod: [github.com/Claudao01/oni-mods/tree/main/mods/fill-with-backwalls](https://github.com/Claudao01/oni-mods/tree/main/mods/fill-with-backwalls).

A configuração é salva em `config.json`. Ao atualizar manualmente, preserve esse arquivo caso queira manter um valor personalizado.

## Compatibilidade

- API de mods do ONI: 2;
- versão mínima declarada do jogo: build 600000;
- jogo base e DLCs: sem restrições declaradas, pendente de validação final em sobrevivência e Spaced Out!.

## Desenvolvimento

O projeto depende das DLLs locais do ONI e usa PLib 4.25.0, incorporada à DLL final por ILRepack. Os avisos e a licença estão em `THIRD_PARTY_NOTICES.md`. Consulte o repositório [Claudao01/oni-mods](https://github.com/Claudao01/oni-mods) para código-fonte, problemas conhecidos e releases.

## Steam Workshop

O texto bilíngue para a página do mod na Steam Workshop está em [docs/STEAM-WORKSHOP.md](./docs/STEAM-WORKSHOP.md).
