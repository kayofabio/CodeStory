
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
                "Um pequeno tropeço",
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
                "Quantidade inválida",
                "A quantidade de maçãs não pode ser negativa."
            );
        }

        if (quantidade == 0)
        {
            return new ResultadoFase(
                "Não estou com fome!",
                "Maçãs para depois",
                "Lia decide guardar as maçãs para outro momento."
            );
        }

        if (quantidade <= 3)
        {
            return new ResultadoFase(
                "Que lanche gostoso!",
                "Um lanche na medida",
                "Lia separou maçãs suficientes para um pequeno lanche."
            );
        }

        return new ResultadoFase(
            "São muitas maçãs!",
            "Vamos guardar algumas",
            "Rosa sugere guardar algumas maçãs para depois."
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
