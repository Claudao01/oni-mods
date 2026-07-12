# Meteor Dig Orders

Solicita automaticamente a escavação de terreno natural que bloqueia construções expostas a chuvas de meteoros no *Oxygen Not Included*.

## Recursos do MVP 0.1.0

- verifica painéis solares ao final de cada fase de uma chuva de meteoros;
- cria ordens nativas de escavação para terreno natural sobreposto à área do painel;
- permite escolher prioridade de 1 a 9 ou Urgente, com padrão 5;
- preserva ordens existentes com prioridade igual ou superior;
- permite processar somente o mundo ativo ou todos os mundos atingidos;
- oferece notificações configuráveis e uma varredura inicial para colônias existentes;
- inclui pt-BR e inglês como fallback.

## Configuração

Abra **Mods > Meteor Dig Orders > Configurar**. As configurações são globais e permitem habilitar painéis solares, escolher prioridade, escopo de mundos e nível de notificações.

## Instalação local

1. Gere o pacote com `oni-mods/scripts/package-meteor-dig-orders.ps1`.
2. Extraia a pasta `meteor-dig-orders` do ZIP em `Documents\Klei\OxygenNotIncluded\mods\local\`.
3. Ative **Meteor Dig Orders** no menu Mods e reinicie o jogo.

## Compatibilidade

- API de mods do ONI: 2;
- nenhuma DLC obrigatória ou proibida;
- projetado para jogo base e conteúdo adicional que use os eventos nativos de chuva de meteoros.

## Desenvolvimento e licença

O código é distribuído sob a licença GPL-3.0-only e atribui copyright a Claudao01. O projeto usa PLib 4.25.0 incorporada à DLL final.

---

# Meteor Dig Orders (English)

Automatically queues native dig errands for natural terrain obstructing buildings exposed to meteor showers in *Oxygen Not Included*.

Version 0.1.0 supports solar panels, configurable priority (1–9 or Urgent), active-world or all-world processing, configurable notifications, and a one-time scan for existing colonies. English is the built-in fallback language and Brazilian Portuguese is included as a translation.

Licensed under GPL-3.0-only. Copyright Claudao01.
