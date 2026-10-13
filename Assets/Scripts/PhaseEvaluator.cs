
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
