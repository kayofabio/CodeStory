
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PythonRunner
{
    private const int TempoLimiteMs = 10000;
    private const string PrefixoResultado = "__UNITY_RESULT__";

    public async Task<PythonExecutionResult> ExecutarAsync(string codigo)
    {
        string pastaTemp = Application.temporaryCachePath;

        string caminhoCodigo = Path.Combine(
            pastaTemp,
            "script_teste.py"
        );

        string caminhoWrapper = Path.Combine(
            Application.streamingAssetsPath,
            "python_wrapper.py"
        );

        try
        {
            File.WriteAllText(
                caminhoCodigo,
                codigo,
                new UTF8Encoding(false)
            );

            if (!File.Exists(caminhoWrapper))
            {
                UnityEngine.Debug.LogError(
                    "Python wrapper não encontrado. Caminho procurado: "
                    + caminhoWrapper
                );

                return Erro(
                    "Arquivo python_wrapper.py não encontrado. " +
                    "Confira o caminho no Console do Unity."
                );
            }

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "\"" + caminhoWrapper + "\" \"" +
                            caminhoCodigo + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true
            };

            info.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";

            using Process processo = new Process();
            processo.StartInfo = info;
            processo.Start();

            Task<string> tarefaSaida =
                processo.StandardOutput.ReadToEndAsync();

            Task<string> tarefaErro =
                processo.StandardError.ReadToEndAsync();

            Task tarefaEspera =
                Task.Run(() => processo.WaitForExit());

            Task concluida = await Task.WhenAny(
                tarefaEspera,
                Task.Delay(TempoLimiteMs)
            );

            if (concluida != tarefaEspera)
            {
                processo.Kill();
                await tarefaEspera;

                return Erro("Tempo limite de execução excedido.");
            }

            await Task.WhenAll(tarefaSaida, tarefaErro);

            string textoSaida = tarefaSaida.Result.TrimEnd();
            string textoErro = tarefaErro.Result.TrimEnd();

            if (processo.ExitCode != 0)
            {
                return Erro(textoErro);
            }

            int inicio = textoSaida.LastIndexOf(
                PrefixoResultado,
                StringComparison.Ordinal
            );

            if (inicio < 0)
            {
                return Erro(
                    string.IsNullOrEmpty(textoErro)
                        ? "O Python não retornou um resultado válido."
                        : textoErro
                );
            }

            string json = textoSaida.Substring(
                inicio + PrefixoResultado.Length
            ).Trim();

            PythonExecutionResult resultado =
                JsonUtility.FromJson<PythonExecutionResult>(json);

            if (resultado == null)
            {
                return Erro("Não foi possível interpretar a resposta do Python.");
            }

            return resultado;
        }
        catch (Exception ex)
        {
            return Erro(
                "Não foi possível executar o Python: " + ex.Message
            );
        }
    }

    private PythonExecutionResult Erro(string mensagem)
    {
        return new PythonExecutionResult
        {
            saida = "",
            erro = mensagem,
            temQuantidade = false,
            quantidade = 0
        };
    }
}
