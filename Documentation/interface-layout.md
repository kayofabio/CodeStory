# Interface CodeStory

A cena editável é `Assets/Scenes/MVP.unity`. O layout usa Canvas com referência 1600×900, divisão aproximada 60/40, console restrito à esquerda e cartões de história à direita. O console e os objetivos aceitam rolagem. Executar desabilita durante a execução; o botão de próxima fase continua ligado ao GameManager existente.

`StoryPresentation` altera título, progresso e cor da coluna conforme a fase. O sprite `childhood` é opcional, para uma arte futura; por enquanto o cenário inicial é reutilizado. Existem duas fases implementadas. Limpar e Dica foram implementados na segunda revisao: Limpar esvazia apenas o editor; Dica mostra orientacao especifica da fase no console, sem modificar o codigo. Nao foram adicionados controles ficticios de audio/configuracoes.

## Ferramentas do editor

- `CodeStory > Aplicar layout aprovado`: reaplica a composição em MVP salva e fora do Play. Reaplicar redefine o código inicial e textos de apresentação. Preserva as referências dos controladores existentes.
- `CodeStory > Validar layout e gerar prévias`: renderiza 1600×900, 1280×720 e 1024×768 em `Temp/CodeStoryValidation`.
- `CodeStory > Testar Python e progressão (Play)`: entra em Play, testa o caminho real por botões e controladores, escreve `play-tests.txt` no mesmo diretório e sai do Play. Requer MVP salva e Python disponível no PATH.

Os arquivos de requisição em Temp são apenas gatilhos locais de uso único para a montagem desta implementação. Não são distribuídos com o projeto.

## Arte

Arquivo: `Assets/Art/Story/nursery-approved.png`. Gerado com a ferramenta integrada ImageGen, importado como sprite com filtro Point e sem compressão. Textos, molduras e botões são objetos Unity, não uma imagem de interface.

Prompt final utilizado:

> Use case: illustration-story. Create a production game background for CodeStory, a children's programming narrative game. Landscape 3:2 composition, beautiful crisp pixel art, cozy nursery with lavender striped walls, cream and peach furniture, pale blue and mint accents. A happy baby with a tiny brown hair curl in a blue onesie sits centered on a soft oval rug, crib and teddy bear to the left, toy blocks and a rocking horse to the right, warm sunny window, smiling sun, star mobile and fluffy clouds. Match the cozy nursery aesthetic of the reference concept in this conversation. Only the nursery illustration filling the entire image. No UI, borders, text, letters, numbers, logos or watermarks. Visible square pixel clusters, restrained shading, charming polished game art. Baby fully visible with breathing room.

## Verificações

- Compilação C# externa com referências do Unity 6000.6.0f1: passou.
- Wrapper Python 3.11.9: passou em saída UTF-8, inteiro, string em quantidade, SyntaxError e NameError.
- `escrever(...)` gera NameError; cena inicial foi preparada para `print(...)`. Não há evidência suficiente para atribuir o erro histórico a essa causa.
- Compilação no próprio Unity: passou.
- Teste automatizado em Play usando TMP_InputField e Button.onClick: passou em saída acentuada, texto literal com tags, bloqueio/desbloqueio de Executar, erro de sintaxe, avanço para fase 2, avaliação de quantidade inteira e fechamento de conclusão.
- Prévias renderizadas pelo Unity e inspecionadas em 1600×900, 1280×720 e 1024×768: sem sobreposição dos painéis nem cortes nos textos iniciais. Em 4:3 a ilustracao mantem a proporcao com recorte lateral centralizado, mantendo o bebe visivel.
- O teste automatizado saiu do Play ao terminar. Não foi feito um build executável nem teste manual de digitação/rolagem por mouse, pois a automação da janela apresentou timeouts.
- O erro Python relatado na conversa anterior não foi reproduzido com print() válido; não foi alterado o PythonRunner nem o wrapper.

## Refinamento após comparação visual

A marca usa Press Start 2P; titulos usam VT323; codigo, console e corpo usam IBM Plex Mono Medium. Fontes baixadas do repositório oficial google/fonts; licenças OFL incluídas em Assets/Art/Fonts. O realce de Python é uma camada visual separada: o campo mantém código literal, inclusive textos com sinais < e >.

Prompt final da arte refinada, usando a imagem aprovada como referência (ImageGen integrado):

> Precise asset extraction from the attached approved game concept. Recreate ONLY the nursery illustration located in the upper right story panel as a standalone landscape 3:2 background asset. Preserve its exact charming crisp detailed pixel-art aesthetic, rich warm peach sunlight, purple striped nursery walls, teddy and crib on the left, big happy laughing baby with arms raised in a blue onesie near the center foreground, toy blocks and rocking horse right, smiling sunny window. Baby should be prominent exactly like the reference, not small. Full illustration bleed to all edges. Remove all interface framing, labels, buttons, text and borders. Do not redesign the art, match the approved illustration as closely as possible.

## Revisao de enquadramento e acabamento

- Divisao 58,5/41,5, margens consistentes e barra de titulo sem caixa externa.
- Molduras geometricas `PixelFrame`, com cantos em degraus, borda interna clara e cores especificas para narrador/objetivo.
- Icones `PixelIcon` desenhados na mesma grade, agrupados nos cabecalhos, botoes e rodapes. Sem objetos decorativos soltos entre paineis.
- `CodeLineNumbers` acompanha linhas reais e continuacoes de quebra visual; fonte e deslocamento vertical seguem o campo de codigo.
- `StoryArtwork` faz recorte UV proporcional, sem esticar o cenario.
- Textos fixos da interface/narrativa sem acentos. A entrada e a saida Python continuam aceitando Unicode e nao sofrem transliteracao.
- Exemplo de codigo separado no cartao do objetivo e atualizado para a fase 2.
- Fontes novas redistribuidas sob OFL, com licencas em Assets/Art/Fonts.

Validacao desta revisao: compilacao no Unity concluida; testes de Play passaram para gutter com 30 linhas, Limpar, Dica, Python Unicode, bloqueio de execucao, erro de sintaxe, fase 2 e conclusao. Previas inspecionadas novamente em 1600x900, 1280x720 e 1024x768.

Ajuste das bordas: removidos adornos escuros dos cantos e sombra deslocada; contorno continuo mais suave com recortes consistentes. Compilacao aprovada e previas 1280x720 e 1024x768 inspecionadas. Sem remontar ou salvar a cena nesta correcao.

## Primeiras palavras da Lia

Na fase 1, o editor inicia apenas com o comentario de convite e uma linha vazia, sem print preenchido. O exemplo no objetivo e a Dica continuam disponiveis como apoio.

Apos saida valida, SceneController mostra a narracao fixa "Lia disse suas primeiras palavras!" e a saida literal do Python no balao sobre a cabeca da Lia. O console conserva a saida completa. O balao usa ajuste de fonte e reticencias para saidas que excedam seu espaco; as regras de avaliacao existentes continuam iguais. A cada tentativa ele e ocultado, evitando fala antiga em erro/saida vazia. Ao mudar de fase ele tambem desaparece; a apresentacao da fase 2 permanece a anterior.

Testes de Play incluem codigo inicial sem print, balao inicialmente oculto, comentario sem saida sem celebracao, separacao entre narrador e fala com acentos, erro sem balao antigo e passagem para fase 2 sem o balao.

Fase 1: o wrapper valida a arvore Python antes de executar. Aceita apenas chamadas diretas de print com argumentos literais (e sep/end literais), comentarios e linhas vazias. Outros comandos retornam FASE1_APENAS_PRINT, exibido no console e narrador como algo que Lia ainda nao aprendeu. Fase 2 permanece sem esta restricao. Validacao: 5 testes Python passaram (13 casos de comandos bloqueados); compilacao C# sem erros. O teste Play foi ampliado, mas nao executado nesta alteracao.
