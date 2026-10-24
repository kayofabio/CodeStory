
public class PhaseEvaluator
{
    private const int LimiteCaracteres = 30;

    public ResultadoFase AvaliarFase1(string saida)
    {
        if (saida.Length > LimiteCaracteres)
        {
            string fala = saida.Substring(0, LimiteCaracteres) + "...";

            return new ResultadoFase(
                fala,
                "Um pequeno tropeco",
                "Lia queria falar muita coisa, mas acabou se enrolando nas palavras."
            );
        }

        return new ResultadoFase(
            saida,
            null,
            null
        );
    }


    public ResultadoFase AvaliarFase2(int quantidade)
    {
        if (quantidade < 0)
        {
            return new ResultadoFase(
                "",
                "Quantidade invalida",
                "A quantidade de macas nao pode ser negativa."
            );
        }

        if (quantidade == 0)
        {
            return new ResultadoFase(
                "Nao estou com fome!",
                "Macas para depois",
                "Lia decide guardar as macas para outro momento."
            );
        }

        if (quantidade <= 3)
        {
            return new ResultadoFase(
                "Que lanche gostoso!",
                "Um lanche na medida",
                "Lia separou macas suficientes para um pequeno lanche."
            );
        }

        return new ResultadoFase(
            "Sao muitas macas!",
            "Vamos guardar algumas",
            "Rosa sugere guardar algumas macas para depois."
        );
    }

}

public class ResultadoFase
{
    public string Fala { get; }
    public string TituloFinal { get; }
    public string MensagemFinal { get; }

    public ResultadoFase(
        string fala,
        string tituloFinal,
        string mensagemFinal)
    {
        Fala = fala;
        TituloFinal = tituloFinal;
        MensagemFinal = mensagemFinal;
    }

    public bool TemFinalAlternativo =>
        !string.IsNullOrEmpty(TituloFinal);
}
