# Fill with Backwalls

Preenche uma cavidade fechada com a parede de fundo, o material e o visual selecionados usando um único clique.

## Recursos

- adiciona a opção **Preencher cavidade** ao seletor de paredes de fundo;
- cria ordens de construção em toda a cavidade fechada;
- substitui o modelo, o material e o visual de paredes de fundo existentes;
- permite limitar o tamanho máximo da cavidade pelo menu **Mods > Fill with Backwalls > Configurar**;
- inclui interface em português do Brasil e inglês.

## Instalação manual

1. Baixe o ZIP mais recente na página de Releases.
2. Extraia a pasta `fill-with-backwalls` em `Documents\Klei\OxygenNotIncluded\mods\local\`.
3. Ative **Fill with Backwalls** no menu Mods e reinicie o jogo quando solicitado.

## Uso

Selecione uma parede de fundo, escolha o modelo e o material, ative **Preencher cavidade** e clique dentro de uma cavidade fechada. O modo permanece ativo até ser desmarcado.

O limite padrão é de 120 células. A configuração é salva em `config.json`. Ao atualizar manualmente, preserve esse arquivo caso queira manter um valor personalizado.

## Compatibilidade

- API de mods do ONI: 2;
- versão mínima declarada do jogo: build 600000;
- jogo base e DLCs: sem restrições declaradas, pendente de validação final em sobrevivência e Spaced Out!.

## Desenvolvimento

O projeto depende das DLLs locais do ONI e usa PLib 4.25.0, incorporada à DLL final por ILRepack. Os avisos e a licença estão em `THIRD_PARTY_NOTICES.md`. Consulte o repositório [Claudao01/oni-mods](https://github.com/Claudao01/oni-mods) para código-fonte, problemas conhecidos e releases.
